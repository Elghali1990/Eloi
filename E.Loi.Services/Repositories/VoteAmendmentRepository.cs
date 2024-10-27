

namespace E.Loi.Services.Repositories;

public class VoteAmendmentRepository : BaseRepository<VoteAmendementResult>, IVoteAmendmentRepository
{
    public VoteAmendmentRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }

    public async Task<ServerResponse> DeleteVoteAsync(DeleteVoteVm vote)
    {
        try
        {
            var voteAmendments = new List<VoteAmendementResult>();
            foreach (var Id in vote.Ids)
            {
                var vote_ = await findAsync(v => v.AmendmentId == Id);
                if (vote_ != null)
                    voteAmendments.Add(vote_);
            }
            _dbSet.RemoveRange(voteAmendments);
            bool result = await _db.SaveChangesAsync() > 0;
            return new ServerResponse(result, result ? "Success" : "Field");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On ${nameof(DeleteVoteAsync)} Vote", nameof(DeleteVoteAsync));
            throw;
        }
    }

    public async Task<VoteAmendementResult> GetVoteAmendment(Guid AmendId)
    {
        try
        {
            var vote = await _dbSet.FirstOrDefaultAsync(v => v.AmendmentId == AmendId);
            return vote!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(GetVoteAmendment)}", nameof(VoteAmendmentRepository));
            throw;
        }
    }

    public async Task<ServerResponse> InsertVoteAsync(VoteVm vote)
    {
        try
        {
            var voteAmendments = new List<VoteAmendementResult>();
            foreach (var Id in vote.Ids)
            {
                var existVotes = _dbSet.Where(vote => vote.AmendmentId == Id).ToList();
                if (existVotes.Count == 0)
                {
                    voteAmendments.Add(new VoteAmendementResult()
                    {
                        Id = Guid.NewGuid(),
                        InFavor = vote.InFavor,
                        Against = vote.Against,
                        Neutral = vote.Neutral,
                        AmendmentId = Id,
                        Result = vote.Result,
                        Observation = vote.Observation!,
                        IsDeleted = false,
                        Suppressed = false,
                        CreatedBy = vote.UserId,
                        CreationDate = DateTime.UtcNow
                    });
                }
            }
            await _dbSet.AddRangeAsync(voteAmendments);
            var response = await _db.SaveChangesAsync() > 0;
            return new ServerResponse(response, response ? "Success" : "Faild");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(InsertVoteAsync)}", nameof(VoteAmendmentRepository));
            throw;
        }
    }
    public async Task<ServerResponse> UpdateVoteAsync(VoteVm vote)
    {
        try
        {
            List<ServerResponse> responses = new();
            foreach (var Id in vote.Ids)
            {
                var nodeAmendments = await findAsync(v => v.AmendmentId == Id);
                if (nodeAmendments != null)
                {
                    nodeAmendments.InFavor = vote.InFavor;
                    nodeAmendments.Against = vote.Against;
                    nodeAmendments.Neutral = vote.Neutral;
                    nodeAmendments.Result = vote.Result;
                    nodeAmendments.Observation = vote.Observation!;
                    nodeAmendments.LastModifiedBy = vote.UserId;
                    nodeAmendments.ModifictationDate = DateTime.UtcNow;
                    _dbSet.Entry(nodeAmendments).State = EntityState.Modified;
                    if (await _db.SaveChangesAsync() > 0)
                        responses.Add(new ServerResponse(true, "Updated"));
                    else
                        responses.Add(new ServerResponse(false, "Fiald"));
                }
            }

            if (responses.Any(r => r.Flag == false))
                return new ServerResponse(false, "Faild");
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error On Update Vote", nameof(VoteAmendmentRepository));
            throw;
        }
    }
}

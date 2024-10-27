namespace E.Loi.Services.Repositories;

public class VoteNodeRepository : BaseRepository<VoteNodeResult>, IVoteNodeRepository
{
    public VoteNodeRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }

    public async Task<ServerResponse> DeleteVoteAsync(DeleteVoteVm vote)
    {
        try
        {
            var voteNodes = new List<VoteNodeResult>();
            foreach (var Id in vote.Ids)
            {
                var vote_ = await findAsync(v => v.NodePhaseLawId == Id);
                if (vote_ != null)
                {
                    voteNodes.Add(vote_);
                }
            }

            _dbSet.RemoveRange(voteNodes);
            bool result = await _db.SaveChangesAsync() > 0;
            return new ServerResponse(result, result ? "Success" : "Field");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error On Insert Vote", nameof(DeleteVoteAsync));
            throw;
        }
    }

    public async Task<ServerResponse> InsertVoteAsync(VoteVm vote)
    {
        try
        {
            var voteNodes = new List<VoteNodeResult>();
            foreach (var Id in vote.Ids)
            {
                var existeVote = await findAsync(v => v.NodePhaseLawId == Id);
                if (existeVote is null)
                {
                    voteNodes.Add(new VoteNodeResult()
                    {
                        Id = Guid.NewGuid(),
                        InFavor = vote.InFavor,
                        Against = vote.Against,
                        Neutral = vote.Neutral,
                        NodePhaseLawId = Id,
                        Result = vote.Result,
                        Observation = vote.Observation!,
                        IsDeleted = false,
                        Suppressed = false,
                        CreatedBy = vote.UserId,
                        CreationDate = DateTime.UtcNow
                    });
                }
            }
            await _dbSet.AddRangeAsync(voteNodes);
            var response = await _db.SaveChangesAsync() > 0;
            return new ServerResponse(response, response ? "Success" : "Faild");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error On Insert Vote", nameof(VoteNodeResult));
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
                var nodeVote = await findAsync(v => v.NodePhaseLawId == Id);
                if (nodeVote != null)
                {
                    nodeVote.InFavor = vote.InFavor;
                    nodeVote.Against = vote.Against;
                    nodeVote.Neutral = vote.Neutral;
                    nodeVote.Result = vote.Result;
                    nodeVote.Observation = vote.Observation!;
                    nodeVote.LastModifiedBy = vote.UserId;
                    nodeVote.ModifictationDate = DateTime.UtcNow;
                    _dbSet.Entry(nodeVote).State = EntityState.Modified;
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
            _logger.LogError(ex.Message, "Error On Update Vote", nameof(VoteNodeResult));
            throw;
        }
    }
}

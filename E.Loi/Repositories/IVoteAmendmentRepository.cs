namespace E.Loi.Repositories;

public interface IVoteAmendmentRepository
{
    Task<ServerResponse> InsertVote(VoteVm vote);
    Task<ServerResponse> UpdateVote(VoteVm vote);
    Task<ServerResponse> DeleteVoteAsync(DeleteVoteVm vote);
    Task<List<VoteDto>> GetVoteAmendmentsAsync(Guid NodeId);
}

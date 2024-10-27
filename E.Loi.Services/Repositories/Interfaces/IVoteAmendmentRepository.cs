namespace E.Loi.Services.Repositories.Interfaces;

public interface IVoteAmendmentRepository : IBaseRepository<VoteAmendementResult>
{
    Task<ServerResponse> InsertVoteAsync(VoteVm vote);
    Task<ServerResponse> UpdateVoteAsync(VoteVm vote);
    Task<ServerResponse> DeleteVoteAsync(DeleteVoteVm vote);
    Task<VoteAmendementResult> GetVoteAmendment(Guid AmendId);
}

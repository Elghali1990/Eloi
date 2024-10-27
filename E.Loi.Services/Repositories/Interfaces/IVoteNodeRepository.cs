namespace E.Loi.Services.Repositories.Interfaces
{
    public interface IVoteNodeRepository : IBaseRepository<VoteNodeResult>
    {
        Task<ServerResponse> InsertVoteAsync(VoteVm vote);
        Task<ServerResponse> UpdateVoteAsync(VoteVm vote);
        Task<ServerResponse> DeleteVoteAsync(DeleteVoteVm vote);
    }
}

namespace E.Loi.Repositories;
public interface IVoteNodeRepository
{
    Task<ServerResponse> InsertVote(VoteVm vote);
    Task<ServerResponse> UpdateVote(VoteVm vote);
    Task<ServerResponse> DeleteVoteAsync(DeleteVoteVm vote);

    Task<List<VoteDto>> getVoteNodesAsync(Guid section);
}

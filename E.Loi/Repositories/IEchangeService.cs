namespace E.Loi.Repositories;
public interface IEchangeService
{
    Task<ServerResponse> sendTeamsToMF(List<TeamDto> teamDtos);
    Task<ServerResponse> getAmendmentsAsync(List<AmendmentDto> amendments, Guid LawIdFinace, string phase);
    Task<ServerResponse> SendVoteAmendmentsAsync(VoteDto[] votes);
    Task<ServerResponse> SendVoteNodesAsync(VoteDto[] votes);
}
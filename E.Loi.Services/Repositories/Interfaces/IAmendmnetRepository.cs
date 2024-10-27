namespace E.Loi.Services.Repositories.Interfaces;

public interface IAmendmnetRepository : IBaseRepository<Amendment>
{
    Task<ServerResponse> CreateAmendmantAsync(AmendmentVm amendment);
    Task<ServerResponse> CheckAmendmentExisteByNumberAsync(Guid LawId, Guid phaseId, List<Guid> TeamIds, int Number);
    Task<AmendmentVm> GetAmendmentByIdAsync(Guid Id);
    Task<List<AmendmentsListVm>> GetAllSubmitedAmendments(Guid NodeId);
    Task<AmendmentDetails> GetAmendmentDetailsAsync(Guid Id);
    Task<ServerResponse> UpdateAmendmantAsync(Guid amendmentId, AmendmentVm amendment);
    Task<ServerResponse> DeleteAmendmantAsync(Guid amendmentId, Guid UserId);
    Task<List<AmendmentsListVm>> GetAmendmentsListAsync(List<Guid> teamIds, Guid nodeId);
    Task<List<AmendmentsListVm>> GetPublicAmendmentsListAsync(Guid nodeId);
    Task<List<AmendmentsListVm>> GetClusterAmendments(Guid AmendmentId);
    Task<List<AmendmentsListVm>> GetAmendmentsPublicAsync(Guid NodeId);
    Task<List<Amendment>> GetAmendmentsForVotingFile(Guid NodeId);
    Task<List<AmendmentsListVm>> GetNodeAmendmentsAsync(Guid NodeId);
    Task<List<Amendment>> GetNodeAmendmentsByNodeId(Guid NodeId);
    Task<List<AmendmentsListVm>> GetAmendmentsListForVotingAsync(Guid nodeId);
    Task<List<AmendmentsListVm>> GetAmendmentsListAsync(List<Guid> Ids);
    Task<ServerResponse> CloneAmendmentsAsync(CloneAmendmentsVm amendments);
    Task<List<AmendmentsListVm>> GetSubmitedAmendmentsListAsync(Guid nodeId);
    Task<List<AmendmentsListVm>> GetLiftAmendments(List<Guid> TeamIds, Guid LawId, Guid PhaseId);
    Task<ServerResponse> SetAmendmentsAsync(SetAmendmentStatuVm amendmentStatuVm);
    Task<ServerResponse> SetNewContent(SetContent model);
    Task<ServerResponse> SetAmendmentsOrders(SetAmendmentOrder model);
    Task<List<AmendmentsListVm>> GetAllSubmitedAmendments(Guid LawId, Guid PhaseLawId);
    Task<List<Amendment>> GetAmendmentsForVoteFileCommision(Guid LawId, Guid PhaseLawId, bool includeAmendmentRatraper);
    Task<ServerResponse> CheckAmendmentsHasNewContent(Guid LawId, Guid PhaseLawId);
    Task<ServerResponse> CheckAmendmentsSectionHasNewContent(Guid nodeId);
    Task<List<AmendmentsListVm>> GetAmendmentsForCluster(Guid NodeId);
    Task<List<AmendmentsListVm>> GetAmendmentsForCluster(Guid LawId, Guid PhaseLawId);
    Task<ServerResponse> insertGovernmentAmendmentsAsync(List<AmendmentDto> amendments);
    Task<List<AmendmentDto>> GetAmendmentsAsync(List<Guid> TeamIds, Guid lawId, Guid PhaseLawId);
    Task<List<VoteAmendment>> GetVoteAmendmentsAsync(Guid NodeId);
    Task<List<CountAmendmentDto>> CountAmendmentsByTeamAndLaw(Guid LawId, Guid PhaseId);
    Task<ServerResponse> ReassignmentAmendment(Guid AmendmentId, Guid NodeId, Guid LastModifiedBy);
    Task<ServerResponse> SetAmendmentsNumbers(List<Guid> TeamIds, Guid LawId, Guid PhaseLawId, Guid LastModifiedBy);
    Task<List<StatisticAmendment>> Statistic(Guid Lawid, Guid PhaseLawId);
    Task<List<VoteResultDto>> GetVoteResult(Guid Lawid, Guid PhaseLawId);
    Task<ServerResponse> ChangeAmendmentTeam(List<Guid> AmendmentIds, Guid TeamId,Guid LastModifiedBy);
    Task<ServerResponse> CheckBeforChangeAmendmentNode(Guid NodeId, Guid AmendmentId);
}

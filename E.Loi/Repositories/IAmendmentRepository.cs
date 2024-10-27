namespace E.Loi.Repositories;
public interface IAmendmentRepository
{
    Task<ServerResponse> CreateAmendmantAsync(AmendmentVm amendment);
    Task<ServerResponse> CheckAmendmentExisteByNumberAsync(Guid LawId, Guid PhaseId, List<Guid> TeamIds, int Number);
    Task<List<AmendmentsListVm>> GetAmendmentsListAsync(List<Guid> teamIds, Guid nodeId);
    Task<List<AmendmentsListVm>> GetPublicAmendmentsListAsync(Guid nodeId);
    Task<List<AmendmentsListVm>> GetAllSubmitedAmendments(Guid NodeId);
    Task<List<AmendmentsListVm>> GetSubmitedAmendmentsListAsync(Guid nodeId);
    Task<AmendmentVm> GetAmendmentByIdAsync(Guid Id);
    Task<ServerResponse> UpdateAmendmantAsync(Guid amendmentId, AmendmentVm amendment);
    Task<ServerResponse> DeleteAmendmantAsync(Guid amendmentId, Guid UserId);
    Task<ServerResponse> SetAmendmentsAsync(SetAmendmentStatuVm amendmentStatuVm);
    Task<List<AmendmentsListVm>> GetAmendmentsPublicAsync(Guid nodeId);
    Task<List<AmendmentsListVm>> GetAmendmentsListForVotingAsync(Guid nodeId);
    Task<List<AmendmentsListVm>> GetClusterAmendments(Guid AmendmentId);
    Task<List<AmendmentsListVm>> GetLiftAmendments(List<Guid> TeamIds, Guid LawId, Guid PhaseId);
    Task<List<AmendmentsListVm>> GetClonedAmendments(Guid TeamId, Guid LawId, Guid PhaseId);
    Task<ServerResponse> CloneAmendmentsAsync(CloneAmendmentsVm amendments);
    Task<AmendmentDetails> GetAmendmentDetailsAsync(Guid Id);
    Task<List<AmendmentsListVm>> GetNodeAmendmentsAsync(Guid NodeId);
    Task<ServerResponse> SetNewContent(SetContent model);
    Task<ServerResponse> SetAmendmentsOrders(SetAmendmentOrder model);
    Task<List<AmendmentsListVm>> GetAllSubmitedAmendments(Guid LawId, Guid PhaseLawId);
    Task<List<AmendmentsListVm>> GetAmendmentsForCluster(Guid NodeId);
    Task<List<AmendmentsListVm>> GetAmendmentsForCluster(Guid LawId, Guid PhaseLawId);
    Task<ServerResponse> CheckAmendmentsHasNewContent(Guid LawId, Guid PhaseLawId);
    Task<ServerResponse> CheckAmendmentsSectionHasNewContent(Guid nodeId);
    Task<List<AmendmentDto>> GetAmendmentsAsync(Guid lawId, Guid PhaseLawId);
    Task<List<AmendmentDto>> GetAmendmentsAsync(List<Guid> TeamIds, Guid lawId, Guid PhaseLawId);
    Task<List<CountAmendmentDto>> CountAmendmentsByTeamAndLaw(Guid lawId, Guid PhaseLawId);
    Task<ServerResponse> ReassignmentAmendment(Guid AmendmentId, Guid NodeId, Guid LastModifiedBy);
    Task<ServerResponse> SetAmendmentsNumbers(List<Guid> TeamIds, Guid LawId, Guid PhaseLawId, Guid LastModifiedBy);
    Task<List<StatisticAmendment>> statistic(Guid lawId, Guid phaseLawId);
    Task<ServerResponse> ChangeAmendmentTeam(List<Guid> AmendmentIds, Guid TeamId, Guid LastModifiedBy);
    Task<ServerResponse> CheckBeforChangeAmendmentNode(Guid NodeId, Guid AmendmentId);
}


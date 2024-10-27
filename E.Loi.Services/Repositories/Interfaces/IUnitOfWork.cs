namespace E.Loi.Services.Repositories.Interfaces;

public interface IUnitOfWork
{
    INodeRepository Nodes { get; }
    ILawRepository Laws { get; }
    IUserRepository Users { get; }
    IAmendmnetRepository Amendmnets { get; }
    ITeamRepository Teams { get; }
    IVoteNodeRepository VoteNodes { get; }
    IVoteAmendmentRepository VoteAmendments { get; }
    INodeTypeRepository NodeTypes { get; }
    ILegislativeRepository Legislatives { get; }
    ILegislativeSessionRepository LegislativeSessions { get; }
    ILegislativeYearRepository LegislativeYears { get; }
    IPhaseRepository Phases { get; }
    IDocumentRepository Documents { get; }
    IRoleRepository Roles { get; }
    IStatisticsRepository Statistics { get; }
    IStatuRepository Status { get; }
    ITraceService Trace { get; }
    Task<bool> CompleteAsync();

}

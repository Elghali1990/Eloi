namespace E.Loi.Services.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LawDbContext _dbContext;
    public INodeRepository Nodes { get; private set; }

    public ILawRepository Laws { get; private set; }

    public IUserRepository Users { get; private set; }

    public IAmendmnetRepository Amendmnets { get; private set; }

    public ITeamRepository Teams { get; private set; }

    public IVoteNodeRepository VoteNodes { get; private set; }

    public IVoteAmendmentRepository VoteAmendments { get; private set; }

    public INodeTypeRepository NodeTypes { get; private set; }

    public ILegislativeRepository Legislatives { get; private set; }

    public ILegislativeSessionRepository LegislativeSessions { get; private set; }

    public ILegislativeYearRepository LegislativeYears { get; private set; }

    public IPhaseRepository Phases { get; private set; }

    public IDocumentRepository Documents { get; private set; }

    public IRoleRepository Roles { get; private set; }

    public IStatisticsRepository Statistics { get; private set; }

    public IStatuRepository Status { get; private set; }

    public ITraceService Trace { get; private set; }

    public UnitOfWork(LawDbContext dbContext, ILoggerFactory _logger)
    {
        _dbContext = dbContext;
        var logger = _logger.CreateLogger("logs");
        Nodes = new NodeRepository(logger, dbContext);
        Laws = new LawRepository(logger, dbContext);
        Users = new UserRepository(logger, dbContext);
        Amendmnets = new AmendmnetRepository(logger, dbContext);
        Teams = new TeamRepository(logger, dbContext);
        VoteNodes = new VoteNodeRepository(logger, dbContext);
        VoteAmendments = new VoteAmendmentRepository(logger, dbContext);
        NodeTypes = new NodeTypeRepository(logger, dbContext);
        Legislatives = new LegislativeRepository(logger, dbContext);
        LegislativeSessions = new LegislativeSessionRepository(logger, dbContext);
        LegislativeYears = new LegislativeYearRepository(logger, dbContext);
        Phases = new PhaseRepository(logger, dbContext);
        Documents = new DocumentRepository(logger, dbContext);
        Roles = new RoleRepository(logger, dbContext);
        Statistics = new StatisticsRepository(logger, dbContext);
        Status = new StatuRepository(logger, dbContext);
        Trace = new TraceService(logger, dbContext);
    }
    public async Task<bool> CompleteAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}

namespace E.Loi.Services.Repositories;

public class LegislativeSessionRepository : BaseRepository<LegislativeSession>, ILegislativeSessionRepository
{
    public LegislativeSessionRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
}

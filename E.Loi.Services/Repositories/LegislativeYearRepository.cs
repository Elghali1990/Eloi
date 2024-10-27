namespace E.Loi.Services.Repositories;

public class LegislativeYearRepository : BaseRepository<LegislativeYear>, ILegislativeYearRepository
{
    public LegislativeYearRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
}

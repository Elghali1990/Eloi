namespace E.Loi.Services.Repositories;

public class LegislativeRepository : BaseRepository<Legislative>, ILegislativeRepository
{
    public LegislativeRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
}


namespace E.Loi.Services.Repositories;

public class PhaseRepository : BaseRepository<Phase>, IPhaseRepository
{
    public PhaseRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
}

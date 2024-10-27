
namespace E.Loi.Services.Repositories;

public class PhaseLawIdRepository : BaseRepository<PhaseLawId>, IPhaseLawIdRepository
{
    public PhaseLawIdRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
}

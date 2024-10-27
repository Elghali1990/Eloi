
namespace E.Loi.Services.Repositories;

public class RoleRepository : BaseRepository<Role>, IRoleRepository
{
    public RoleRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
}

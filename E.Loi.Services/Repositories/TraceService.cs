
namespace E.Loi.Services.Repositories;

public class TraceService : BaseRepository<Trace>, ITraceService
{
    public TraceService(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
}

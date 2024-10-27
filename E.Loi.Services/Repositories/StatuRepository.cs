

namespace E.Loi.Services.Repositories;

public class StatuRepository : BaseRepository<Statut>, IStatuRepository
{
    public StatuRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }

    public async Task<List<Statut>> GetAllStatusWithLaws()
    {
        try
        {
            // var status =await _dbSet.Include(s => s.Laws).ToListAsync();
            var status = await _dbSet.ToListAsync();
            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(GetAllStatusWithLaws)}", nameof(StatuRepository));
            throw;
        }
    }

    public async Task<Statut> getByOrder(int order)
    {
        try
        {
            return (await _dbSet.FirstOrDefaultAsync(s => s.Order == order))!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(GetAllStatusWithLaws)}", nameof(StatuRepository));
            throw;
        }
    }
}

using System.Linq.Expressions;

namespace E.Loi.Services.Repositories;

public class BaseRepository<Table> : IBaseRepository<Table> where Table : class
{
    public readonly ILogger _logger;
    public readonly LawDbContext _db;
    internal DbSet<Table> _dbSet;

    public BaseRepository(ILogger logger, LawDbContext db)
    {
        _logger = logger;
        _db = db;
        _dbSet = _db.Set<Table>();
    }
    public virtual async Task<bool> CreateAsync(Table table)
    {
        await _dbSet.AddAsync(table);
        return await _db.SaveChangesAsync() > 0;
    }

    public virtual Task<Table> DeleteAsync(Table table)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<Table> findAsync(Expression<Func<Table, bool>> expression)
    {
        return (await _dbSet.FirstOrDefaultAsync(expression))!;
    }

    public virtual async Task<List<Table>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().AsSplitQuery().ToListAsync();
    }

    public virtual async Task<Table> GetByIdAsync(Guid Id)
    {
        return (await _dbSet.FindAsync(Id))!;
    }

    public async virtual Task<List<Table>> getWithOptions(Expression<Func<Table, bool>> expression)
    {
        return await _dbSet.Where(expression).ToListAsync();
    }

    public virtual async Task<bool> UpdateAsync(Table table)
    {
        _db.Entry(table).State = EntityState.Modified;
        return await _db.SaveChangesAsync() > 0;

    }
}

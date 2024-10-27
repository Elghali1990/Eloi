using System.Linq.Expressions;

namespace E.Loi.Services.Repositories.Interfaces;

public interface IBaseRepository<Table> where Table : class
{
    Task<bool> CreateAsync(Table table);
    Task<bool> UpdateAsync(Table table);
    Task<Table> DeleteAsync(Table table);
    Task<List<Table>> GetAllAsync();
    Task<Table> GetByIdAsync(Guid Id);
    Task<Table> findAsync(Expression<Func<Table, bool>> expression);
    Task<List<Table>> getWithOptions(Expression<Func<Table, bool>> expression);
}

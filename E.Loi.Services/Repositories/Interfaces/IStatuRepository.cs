namespace E.Loi.Services.Repositories.Interfaces;

public interface IStatuRepository : IBaseRepository<Statut>
{
    Task<List<Statut>> GetAllStatusWithLaws();
    Task<Statut> getByOrder(int order);
}

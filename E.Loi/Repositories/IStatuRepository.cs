namespace E.Loi.Repositories;

public interface IStatuRepository
{
    Task<Statut> getStatusByOrder(int order);
    Task<List<Statut>> getAllStatus();
    Task<List<Statut>> getStatusByPhaseId(Guid PhaseId, int Order);
    Task<Statut> getStatusById(Guid Id);
}

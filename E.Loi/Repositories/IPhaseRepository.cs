namespace E.Loi.Repositories;

public interface IPhaseRepository
{
    Task<List<Phase>> getAllAsync();
    Task<Phase> getPhaseById(Guid Id);
    Task<Phase> getPhaseByOrder(int Order);
}

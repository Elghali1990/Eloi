namespace E.Loi.Repositories;

public interface ILegislativeRepository
{
    Task<List<Legislative>> getAll();
}

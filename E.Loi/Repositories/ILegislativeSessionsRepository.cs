namespace E.Loi.Repositories;

public interface ILegislativeSessionsRepository
{
    Task<List<LegislativeSession>> GetAllByIdYear(string IdYear);
}

namespace E.Loi.Repositories;

public interface ILegislativeYearsRepository
{
    Task<List<LegislativeYear>> GetLegislativeYearsByIdLegislative(string LegislativeId);
}

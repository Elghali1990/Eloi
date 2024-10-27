namespace E.Loi.Repositories;

public interface IStatisticsRepository
{
    Task<List<StatisticsVM>> StatisticsByCommittees();
    Task<List<StatisticsVM>> StatisticsByParliamentaryTeams();
    Task<List<StatisticsDtos>> StatisticsReadOne();
    Task<List<StatisticsDtos>> StatisticsReadTwo();
    Task<List<StatisticsDtos>> StatisticsReadOne(SearchDtos search);
    Task<List<StatisticsDtos>> StatisticsReadTwo(SearchDtos search);
}

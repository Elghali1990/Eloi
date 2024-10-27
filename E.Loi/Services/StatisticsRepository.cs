namespace E.Loi.Services;

public class StatisticsRepository(IHttpClientFactory _clientFactory) : IStatisticsRepository
{
    HttpClient http = _clientFactory.CreateClient("api");
    public async Task<List<StatisticsVM>> StatisticsByCommittees()
    {
        try
        {
            var response = await http.GetAsync("Statistics/statisticsByCommittees");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<StatisticsVM>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<StatisticsVM>> StatisticsByParliamentaryTeams()
    {
        try
        {
            var response = await http.GetAsync("Statistics/statisticsByParliamentaryTeams");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<StatisticsVM>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<StatisticsDtos>> StatisticsReadOne()
    {
        try
        {
            var response = await http.GetAsync("Statistics/getStatisticReadOne");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<StatisticsDtos>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<StatisticsDtos>> StatisticsReadOne(SearchDtos search)
    {
        try
        {
            var response = await http.PostAsJsonAsync("Statistics/filterStatisticReadOne", search);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<StatisticsDtos>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<StatisticsDtos>> StatisticsReadTwo()
    {
        try
        {
            var response = await http.GetAsync("Statistics/getStatisticReadTwo");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<StatisticsDtos>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<StatisticsDtos>> StatisticsReadTwo(SearchDtos search)
    {
        try
        {
            var response = await http.PostAsJsonAsync("Statistics/filterStatisticReadTwo", search);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<StatisticsDtos>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}

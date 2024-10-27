namespace E.Loi.Services;

public class LegislativeSessionsRepository(IHttpClientFactory _clientFactory) : ILegislativeSessionsRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task<List<LegislativeSession>> GetAllByIdYear(string IdYear)
    {
        try
        {
            var response = await _httpClient.GetAsync($"LegislativeSessions/GetAllLegislativeSessionsByIdYear/{IdYear}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LegislativeSession>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


}

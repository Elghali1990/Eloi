
namespace E.Loi.Services;

public class LegislativeYearsRepository(IHttpClientFactory _clientFactory) : ILegislativeYearsRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task<List<LegislativeYear>> GetLegislativeYearsByIdLegislative(string LegislativeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"LegislativeYears/GetAllLegislativeYearsByIdLegislative/{LegislativeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LegislativeYear>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

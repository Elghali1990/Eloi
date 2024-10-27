namespace E.Loi.Services;

public class LegislativeRepository(IHttpClientFactory _clientFactory) : ILegislativeRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task<List<Legislative>> getAll()
    {
        try
        {
            var response = await _httpClient.GetAsync("Legislatives/GetAllLegislatives");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<Legislative>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}

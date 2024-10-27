namespace E.Loi.Services;

public class PhaseRepository(IHttpClientFactory _clientFactory) : IPhaseRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task<List<Phase>> getAllAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Phases/getAllPhase");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<Phase>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Phase> getPhaseById(Guid Id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Phases/getPhaseById/{Id}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Phase>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Phase> getPhaseByOrder(int Order)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Phases/getPhaseByOrder/{Order}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Phase>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}

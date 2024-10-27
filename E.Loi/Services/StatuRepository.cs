namespace E.Loi.Services;

public class StatuRepository(IHttpClientFactory _clientFactory) : IStatuRepository
{
    HttpClient _http = _clientFactory.CreateClient("api");
    public async Task<List<Statut>> getAllStatus()
    {
        try
        {
            var response = await _http.GetAsync("Status/getStatus");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<Statut>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Statut> getStatusById(Guid Id)
    {
        try
        {
            var response = await _http.GetAsync($"Status/getStatusById/{Id}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Statut>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Statut> getStatusByOrder(int order)
    {
        try
        {
            var response = await _http.GetAsync($"Status/getStatusByOrder/{order}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Statut>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<Statut>> getStatusByPhaseId(Guid PhaseId, int Order)
    {
        try
        {
            var response = await _http.GetAsync($"Status/getStatuByPhaseId/{PhaseId}/{Order}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<Statut>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}

namespace E.Loi.Services;

public class TeamRepository(IHttpClientFactory _clientFactory) : ITeamRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    HttpClient _httpClientEchange = _clientFactory.CreateClient("Echange");
    public async Task<ServerResponse> CreateTeamAsync(TeamVm vm, Guid CreatedBy)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Team/CreateTeamAsync/{CreatedBy}", vm);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> DelteTeamAsync(Guid Id, Guid LastModifiedBy)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Team/DelteTeamAsync/{Id}/{LastModifiedBy}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<TeamVm>> GetAll()
    {
        try
        {
            var response = await _httpClient.GetAsync("Team/GetAll");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<TeamVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<TeamVm>> GetCommissionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Team/getCommissionsAsync");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<TeamVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<TeamVm>> GetAllTeamsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Team/getAllTeams");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<TeamVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> UpdateTeamAsync(TeamVm vm, Guid LastModifiedBy)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"Team/UpdateTeamAsync/{LastModifiedBy}", vm);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<List<TeamVm>> GetAllTeamsForEchange()
    {
        try
        {
            var response = await _httpClient.GetAsync("Team/getAllTeamsForEchange");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<TeamVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<List<TeamDto>> GetSelecteTeamsForEchange(List<Guid> Ids, Guid LawId)
    {
        try
        {
            var response = await _httpClientEchange.PostAsJsonAsync($"Teams/getAllAsync/{LawId}", Ids);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<TeamDto>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}

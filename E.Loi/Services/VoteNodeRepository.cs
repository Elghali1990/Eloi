namespace E.Loi.Services;

public class VoteNodeRepository(IHttpClientFactory _clientFactory) : IVoteNodeRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    HttpClient _httpClientEchange = _clientFactory.CreateClient("Echange");
    public async Task<ServerResponse> DeleteVoteAsync(DeleteVoteVm vote)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("VoteNodes/DeleteVote", vote);
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<List<VoteDto>> getVoteNodesAsync(Guid section)
    {
        try
        {
            var response = await _httpClientEchange.GetAsync($"Vote/getVoteNodesAsync/{section}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<VoteDto>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<ServerResponse> InsertVote(VoteVm vote)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("VoteNodes/InsertVote", vote);
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> UpdateVote(VoteVm vote)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("VoteNodes/InsertVote", vote);
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}

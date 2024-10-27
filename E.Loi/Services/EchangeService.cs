namespace E.Loi.Services;

public class EchangeService(IHttpClientFactory httpClientFactory) : IEchangeService
{
    HttpClient httpClient = httpClientFactory.CreateClient("UrlFinance");

    public async Task<ServerResponse> getAmendmentsAsync(List<AmendmentDto> amendments, Guid LawIdFinace, string phase)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"getAmendmentsAsync/{LawIdFinace}/{phase}", amendments);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<ServerResponse> sendTeamsToMF(List<TeamDto> teamDtos)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("getAllTeamsAsync", teamDtos);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<ServerResponse> SendVoteAmendmentsAsync(VoteDto[] votes)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("sendVoteAmendmentsAsync", votes);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<ServerResponse> SendVoteNodesAsync(VoteDto[] votes)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync($"sendVoteNodesAsync", votes);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}

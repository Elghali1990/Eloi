
namespace E.Loi.Services;

public class RoleRepository(IHttpClientFactory _clientFactory) : IRoleRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task<List<Role>> GetAll()
    {
        try
        {
            var response = await _httpClient.GetAsync("Roles/getAllAsync");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<Role>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}

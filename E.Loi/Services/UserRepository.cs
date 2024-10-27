namespace E.Loi.Services;

public class UserRepository(IHttpClientFactory _clientFactory) : IUserRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task<ServerResponse> CreateUserAsync(UserVm userVm, Guid CreatedBy)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Users/createuserAsync/{CreatedBy}", userVm);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return new ServerResponse(false, "Faild");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<string> DecryptToken(string jWTToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Users/DecryptToken?token={jWTToken}");
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadAsStringAsync())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> DeleteUserAsync(Guid UserId, Guid LastModifiedBy)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Users/deleteUserAsync/{UserId}/{LastModifiedBy}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<UserVm>> getAllUsers()
    {
        try
        {
            var response = await _httpClient.GetAsync("Users/GetAllUserAsync");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<UserVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<UserVm> GetUserByIdAsync(Guid Id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Users/getUserByAsync/{Id}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<UserVm>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<UserVm> login(LoginVm loginVm)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Users/login", loginVm);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode.ToString() != "OK")
                    return null!;
                return (await response.Content.ReadFromJsonAsync<UserVm>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<string> LoginGenerateTockenAsync(LoginVm loginVm)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Users/LoginGenerateTockenAsync", loginVm);
            if (response.IsSuccessStatusCode)
            {
                return (await response.Content.ReadAsStringAsync())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> UpdateUserAsync(UserVm userVm, Guid LastModifiedBy)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"Users/updateuserAsync/{LastModifiedBy}", userVm);
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

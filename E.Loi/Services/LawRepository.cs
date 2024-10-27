
namespace E.Loi.Services;

public class LawRepository(IHttpClientFactory _clientFactory) : ILawRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    HttpClient _httpLawApi = _clientFactory.CreateClient("lawApi");
    public async Task<Law> AddLawAsync(EditLawVm model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Law/InsertLaw", model);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Law>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> CheckLawExiste(string Number, int Year)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/checkLawExiste/{Number}/{Year}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return new ServerResponse(false, "Fail");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> DeleteLawAsync(Guid LawId, Guid LastModifiedBy)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Law/deleteLawAsync/{LawId}/{LastModifiedBy}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return new ServerResponse(false, "Fail");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetAllLawsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Law/GetAllLawsAsync");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetAllLawsCommissionAsync(Guid IdCommission)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/GetAllLawsCommissionAsync/{IdCommission}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> getAllLawsForAll()
    {
        try
        {
            var response = await _httpClient.GetAsync("Law/getAllLawsForAll");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetAllLawsTeamAsync(string actionName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/{actionName}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawListDto>> getAllLawsWithPhases()
    {
        try
        {
            var response = await _httpClient.GetAsync("Law/getAllLawsWithPhases");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawListDto>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetAllPreparationLawsAsync(Guid PhaseId, Guid TeamId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/GetAllPreparationLawsAsync/{PhaseId}/{TeamId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<List<LawVm>> getAllReferences()
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/getAllReferences");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<Law> GetByIdAsync(Guid Id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/getLawById?lawId={Id}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Law>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetCGILawAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Law/GetCGILawAsync");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawDetail>> GetLawByIds(List<Guid> Ids)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Law/getLawByIds", Ids);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawDetail>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Law> getLawByYearAsync(int year)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/getLawByYear/{year}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Law>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<LawInfo> GetLawInfoAsync(Guid LawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/getLawInfo/{LawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<LawInfo>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetLawsByCategoryAsync(string category)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/GetLawsByCategoryAsync/{category}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawDetail>> GetLawsByCommissionId(Guid commissionId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/GetLawByIdCommission/{commissionId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawDetail>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawDetail>> GetLawsByTeamId(Guid teamId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/GetLawsByTeamId/{teamId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawDetail>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetLawsForCommission(List<Guid> EntitiesId, Guid userId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Law/GetLawsForCommission/{userId}", EntitiesId);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetLawsForDirector(List<Guid> EntitiesId, Guid userId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Law/GetLawsForDirector/{userId}", EntitiesId);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetLawsForlegislation()
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/GetLawsForlegislation");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> getLawsForReadTwo(Guid phaseLawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/getLawsForReadTwo/{phaseLawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetLawsForSession(List<Guid>? EntitiesId, Guid userId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Law/GetLawsForSession/{userId}", EntitiesId);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> GetLawStatuAsync(Guid LawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/GetLawStatuAsync/{LawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return new ServerResponse(false, "Fail");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetLawsToPrint(Guid ComId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Law/getLawsToPrint/{ComId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetPlfLawAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Law/getPlfLaws");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<LawVm>> GetPreparationLawsAsync(List<Guid> TeamsId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Law/GetPreparationMLawsAsync", TeamsId);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<LawVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<TextLaw>> GetTextLawFromFile(DocumentTexteLoiVM model)
    {
        try
        {
            var response = await _httpLawApi.PostAsJsonAsync("arborescence/GetArborescenceByIdTexteLoi", model);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<TextLaw>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> setDateIhalaLaw(LawDate lawDate)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Law/setDateIhalaLaw", lawDate);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> SetLawInfo(LawInfo lawInfo)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("Law/setLawInfo", lawInfo);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return new ServerResponse(false, "Fail");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> SetLawStatuAsync(LawStatuVm statuVm)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Law/SetLawStatuAsync", statuVm);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> SetPhaseLawAsync(Guid LawId, Guid PhaseLawId, Guid LastModifiedBy)
    {
        try
        {
            var response = await _httpClient.PutAsync($"Law/SetPhaseLawAsync/{LawId}/{PhaseLawId}/{LastModifiedBy}", null);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return new ServerResponse(false, "Fail");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}




using E.Loi.Components.Pages.Teams;

namespace E.Loi.Services;
public class AmendmentRepository(IHttpClientFactory _clientFactory) : IAmendmentRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    HttpClient _httpClientEchange = _clientFactory.CreateClient("Echange");
    public async Task<ServerResponse> CloneAmendmentsAsync(CloneAmendmentsVm amendments)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Amendment/CloneAmendmentsAsync", amendments);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode.ToString() != "OK")
                    return null!;
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> CreateAmendmantAsync(AmendmentVm amendment)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Amendment/createAmendment", amendment);
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
    public async Task<ServerResponse> DeleteAmendmantAsync(Guid amendmentId, Guid UserId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Amendment/DeleteAmendmantAsync/{amendmentId}/{UserId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetAllSubmitedAmendments(Guid NodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAllSubmitedAmendments/{NodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<AmendmentVm> GetAmendmentByIdAsync(Guid Id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAmendmentByIdAsync/{Id}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode.ToString() != "OK")
                    return null!;
                return (await response.Content.ReadFromJsonAsync<AmendmentVm>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<AmendmentDetails> GetAmendmentDetailsAsync(Guid Id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAmendmentDetailsAsync/{Id}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<AmendmentDetails>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsListAsync(List<Guid> teamIds, Guid nodeId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Amendment/GetAmendmentsListAsync/{nodeId}", teamIds);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsListForVotingAsync(Guid nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAmendmentsListForVotingAsync/{nodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsPublicAsync(Guid nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAmendmentsPublicAsync/{nodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetClonedAmendments(Guid TeamId, Guid LawId, Guid PhaseId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetClonedAmendments/{TeamId}/{LawId}/{PhaseId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetClusterAmendments(Guid AmendmentId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetClusterAmendments/{AmendmentId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetLiftAmendments(List<Guid> TeamIds, Guid LawId, Guid PhaseId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Amendment/GetLiftAmendments/{LawId}/{PhaseId}", TeamIds);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetNodeAmendmentsAsync(Guid NodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetNodeAmendmentsAsync/{NodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetPublicAmendmentsListAsync(Guid nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetPublicAmendmentsListAsync/{nodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetSubmitedAmendmentsListAsync(Guid nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetSubmitedAmendmentsListAsync/{nodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    public async Task<ServerResponse> SetAmendmentsAsync(SetAmendmentStatuVm amendmentStatuVm)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("Amendment/SetAmendmentsAsync", amendmentStatuVm);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetAllSubmitedAmendments(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAllSubmitedAmendments/{LawId}/{PhaseLawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> SetAmendmentsOrders(SetAmendmentOrder model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Amendment/SetAmendmentsOrders", model);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> SetNewContent(SetContent model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Amendment/SetNewContent", model);
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
    public async Task<ServerResponse> UpdateAmendmantAsync(Guid amendmentId, AmendmentVm amendment)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"Amendment/UpdateAmendmantAsync/{amendmentId}", amendment);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> CheckAmendmentsHasNewContent(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/CheckAmendmentsHasNewContent/{LawId}/{PhaseLawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsForCluster(Guid NodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAmendmentsForCluster/{NodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> CheckAmendmentsSectionHasNewContent(Guid nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/checkAmendmentsSectionHasNewContent/{nodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> CheckAmendmentExisteByNumberAsync(Guid LawId, Guid PhaseId, List<Guid> TeamIds, int Number)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Amendment/checkAmendmentExisteByNumberAsync/{LawId}/{PhaseId}/{Number}", TeamIds);
            if (response.IsSuccessStatusCode)
            {
                if (!response.IsSuccessStatusCode)
                    return null!;
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);

        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsForCluster(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAmendmentsForCluster/{LawId}/{PhaseLawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentsListVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentDto>> GetAmendmentsAsync(Guid lawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/GetAmendmentsForCluster/{lawId}/{PhaseLawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentDto>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<AmendmentDto>> GetAmendmentsAsync(List<Guid> TeamIds, Guid lawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _httpClientEchange.PostAsJsonAsync($"Amendments/getAmendmentsAsync/{lawId}/{PhaseLawId}", TeamIds);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<AmendmentDto>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<CountAmendmentDto>> CountAmendmentsByTeamAndLaw(Guid lawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/countAmendmentsByTeamAndLaw/{lawId}/{PhaseLawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<CountAmendmentDto>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> ReassignmentAmendment(Guid AmendmentId, Guid NodeId, Guid LastModifiedBy)
    {

        try
        {
            var response = await _httpClient.GetAsync($"Amendment/ReassignmentAmendment/{AmendmentId}/{NodeId}/{LastModifiedBy}");
            if (response.IsSuccessStatusCode)
            {
                if (!response.IsSuccessStatusCode)
                    return null!;
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);

        }
    }

    public async Task<ServerResponse> SetAmendmentsNumbers(List<Guid> TeamIds, Guid LawId, Guid PhaseLawId, Guid LastModifiedBy)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Amendment/SetAmendmentsNumbers/{LawId}/{PhaseLawId}/{LastModifiedBy}", TeamIds);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode.ToString() != "OK")
                    return null!;
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            }
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<List<StatisticAmendment>> statistic(Guid lawId, Guid phaseLawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/statistic/{lawId}/{phaseLawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<StatisticAmendment>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<ServerResponse> ChangeAmendmentTeam(List<Guid> AmendmentIds, Guid TeamId, Guid LastModifiedBy)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Amendment/changeAmendmentTeam/{TeamId}/{LastModifiedBy}", AmendmentIds);
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

    public async Task<ServerResponse> CheckBeforChangeAmendmentNode(Guid NodeId, Guid AmendmentId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Amendment/checkBeforChangeAmendmentNode/{NodeId}/{AmendmentId}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode.ToString() != "OK")
                    return null!;
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
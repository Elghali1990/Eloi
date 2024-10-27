using Microsoft.AspNetCore.Mvc;

namespace E.Loi.Services;
public class NodeRepository(IHttpClientFactory _clientFactory) : INodeRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task<ServerResponse> SetPhaseNodes(Guid LawId, Guid DestinationPhase, Guid LastModifiedBy, int Order)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/SetPhaseNodes/{LawId}/{DestinationPhase}/{LastModifiedBy}/{Order}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> CreateLawNodes(List<TextLaw> texts, Guid LawId, Guid CreatedBy)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Nodes/CreateLawNodes/{LawId}/{CreatedBy}", texts);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Node> CreateNode(CreateNodeVm model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Nodes/CreateNode", model);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Node>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Node> CreateVirtuelNode(CreateNodeVm model)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Nodes/CreateVirtualNode", model);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Node>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> DeleteNode(Guid nodeId, Guid userId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Nodes/DeleteNode/{nodeId}/{userId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<NodeCounter>> GetDirecteChilds(Guid ParentId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/GetDirecteChilds/{ParentId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<NodeCounter>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<NodeVoteVm[]> GetFlatNodes(Guid? IdLaw, Guid? PhaseLawId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/GetFlatNodes/{IdLaw}/{PhaseLawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<NodeVoteVm[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<FlatNode[]> GetFlatParents(Guid? nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/GetFlatParents/{nodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<FlatNode[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Node> GetNodeByIdAsync(Guid Id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/GetNodeByIdAsync/{Id}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Node>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<NodeContentVm> GetNodeContent(Guid NodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/getNodeContent/{NodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<NodeContentVm>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<NodeVm>> GetRecursiveChildren(Guid IdLaw, Guid PhaseId, bool includeVirtuelNode)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/getNodeChildrens?lawId={IdLaw}&phaseId={PhaseId}&includeVirtuelNode={includeVirtuelNode}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<NodeVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> UpdateNodeContent([FromBody] NodeContentVm nodeContent, Guid UserId)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"Nodes/UpdateNodeContent/{UserId}", nodeContent);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> CloneNodes(Guid LawId, Guid CurentPhase, Guid DestinationPhase, string Statu)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/CloneNodes/{LawId}/{CurentPhase}/{DestinationPhase}/{Statu}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<FlatNode[]> GetLawSections(Guid Lawid, Guid PhaseLawId, bool IsPrinVotingFile)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/getLawSections/{Lawid}/{PhaseLawId}/{IsPrinVotingFile}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<FlatNode[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> cloneSectionWithChildrens(Guid sectionId, Guid destinationPhase)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/cloneSectionWithChildrens/{sectionId}/{destinationPhase}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<Node> GetNodeByID(Guid Id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/getNodeByID/{Id}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<Node>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> SetNewContent(UpdateNodeContent Node)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Nodes/setNewContent", Node);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> GetNodeByTypeByNumberByBis(Guid ParentNodeId,Guid TypeId, int Number, int Bis)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Nodes/getNodeByTypeByNumberByBis/{ParentNodeId}/{TypeId}/{Number}/{Bis}");
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



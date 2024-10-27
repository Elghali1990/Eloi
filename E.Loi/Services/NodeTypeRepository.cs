namespace E.Loi.Services;

public class NodeTypeRepository(IHttpClientFactory _clientFactory) : INodeTypeRepository
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task<NodeType> GetNodeTypeByIdAsync(Guid nodeId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"NodeTypes/GetNodeTypeByIdAsync/{nodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<NodeType>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<NodeType> GetNodeTypeByNameAsync(string label)
    {
        try
        {
            var response = await _httpClient.GetAsync($"NodeTypes/GetNodeTypeByNameAsync/{label}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<NodeType>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }

    }
    public async Task<List<NodeTypeVm>> GetNodeTypesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("NodeTypes/GetAllNodeTypesAsync");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<NodeTypeVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<NodeTypeVm> GetNodeTypesWithNodeHierarchies(Guid ParentId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"NodeTypes/getNodeTypesWithNodeHierarchies/{ParentId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<NodeTypeVm>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}
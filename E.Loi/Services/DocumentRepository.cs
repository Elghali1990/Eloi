namespace E.Loi.Services;

public class DocumentRepository(IHttpClientFactory _clientFactory) : IDocumentRepository
{
    HttpClient _http = _clientFactory.CreateClient("api");
    public async Task<ServerResponse> DeleteDocumentAsync(Guid Id)
    {
        try
        {
            var response = await _http.DeleteAsync($"Documents/DeleteDocumentAsync/{Id}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<ServerResponse>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<DocumentVm> GetDocumentAsync(Guid LawId, string Type)
    {
        try
        {
            var response = await _http.GetAsync($"Documents/GetDocumentAsync/{LawId}/{Type}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<DocumentVm>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<DocumentVm> GetDocumentByIdAsync(Guid docId)
    {
        try
        {
            var response = await _http.GetAsync($"Documents/GetDocumentByIdAsync/{docId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<DocumentVm>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<List<DocumentVm>> GetLawDocumentsAsync(Guid LawId)
    {
        try
        {
            var response = await _http.GetAsync($"Documents/GetLawDocumentsAsync/{LawId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<List<DocumentVm>>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<ServerResponse> InsertDocument(List<DocumentVm> documents)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("Documents/insertDocument", documents);
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

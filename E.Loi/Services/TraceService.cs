namespace E.Loi.Services;

public class TraceService(IHttpClientFactory _clientFactory) : ITraceService
{
    HttpClient _httpClient = _clientFactory.CreateClient("api");
    public async Task insertTrace(Trace trace)
    {
        try
        {
            await _httpClient.PostAsJsonAsync("Trace/insertTrace", trace);
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}

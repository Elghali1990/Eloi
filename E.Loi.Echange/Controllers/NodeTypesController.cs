namespace E.Loi.Echange.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NodeTypesController(IUnitOfWork _uof, Logger<NodeTypesController> _logger, IHttpClientFactory _httpClientFactory) : ControllerBase
{
    HttpClient _httpClient = _httpClientFactory.CreateClient("BaseUrlFinance");
    [HttpGet]
    [Route("insertNodeTypesAsync")]
    public async Task<ActionResult> InsertNodeTypesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"url");
            if (response.IsSuccessStatusCode)
            {
                var nodeTypes = await response.Content.ReadFromJsonAsync<List<NodeTypeDto>>();
                var result = await _uof.NodeTypes.InsertNodeTypesAsync(nodeTypes ?? new List<NodeTypeDto>());
                return Ok(result);
            }
            return BadRequest(response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(InsertNodeTypesAsync)}", nameof(NodeTypesController));
            return BadRequest("Error when insert node types");
        }
    }

}

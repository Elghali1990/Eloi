namespace E.Loi.Echange.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NodesController(IUnitOfWork _uof, ILogger<NodesController> _logger, IHttpClientFactory _clientFactory) : ControllerBase
{
    HttpClient _httpClient = _clientFactory.CreateClient("BaseUrlFinance");

    //[HttpGet]
    //[Route("getNodesByLawIdPhaseLawId/{LawId:guid}/{PhaseLawId:guid}")]
    //public async Task<IActionResult> getNodesByLawIdPhaseLawId(Guid lawId, Guid phaseLawId)
    //{
    //    try
    //    {
    //        var response = await _httpClient.GetAsync($"/{lawId}/{phaseLawId}");
    //        if (response.IsSuccessStatusCode)
    //        {
    //            var lawDto = (await response.Content.ReadFromJsonAsync<LawDto>())!;
    //            //var result = await _uof.Nodes.CreateLawNodes(lawDto: lawDto);
    //            return Ok();
    //        }
    //        return BadRequest(response.StatusCode);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.LogError(ex.Message, $"Erron when colling {nameof(getNodesByLawIdPhaseLawId)}", nameof(NodeTypesController));
    //        return BadRequest("Error when get nodes by LawId&phaseLawId");
    //    }
    //}

    //depot law


    [HttpPost]
    [Route("createNodesAsync/{LawId:guid}")]
    public async Task<IActionResult> createNodesAsync([FromBody] NodeDto[] NodeDtos, Guid LawId)
    {
        try
        {
            var response = await _uof.Nodes.CreateNodesLawAsync(NodeDtos, LawId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(createNodesAsync)}", nameof(NodeTypesController));
            return BadRequest("Error when create law and nodes");
        }
    }

}

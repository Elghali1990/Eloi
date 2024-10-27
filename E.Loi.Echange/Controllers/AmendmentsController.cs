namespace E.Loi.Echange.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AmendmentsController(IUnitOfWork _uof, ILogger<AmendmentsController> _logger, IHttpClientFactory _clientFactory) : ControllerBase
{
    HttpClient _httpClient = _clientFactory.CreateClient("BaseUrlFinance");

    [HttpPost]
    [Route("insertGovernmentAmendmentsAsync")]
    public async Task<IActionResult> InsertGovernmentAmendmentsAsync([FromBody] List<AmendmentDto> amendments)
    {
        try
        {
            var response = await _uof.Amendmnets.insertGovernmentAmendmentsAsync(amendments);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(InsertGovernmentAmendmentsAsync)}", nameof(AmendmentsController));
            return BadRequest(ex.Message);
        }
    }



    [HttpPost]
    [Route("getAmendmentsAsync/{LawId:guid}/{PhaseLawId:guid}")]
    public async Task<IActionResult> GetAmendmentsAsync(List<Guid> TeamIds, Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _uof.Amendmnets.GetAmendmentsAsync(TeamIds, LawId, PhaseLawId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(GetAmendmentsAsync)}", nameof(AmendmentsController));
            return BadRequest(ex.Message);
        }
    }




}

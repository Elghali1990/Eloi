namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LegislativeSessionsController(IUnitOfWork _unitofwork, ILogger<LegislativeSessionsController> _logger) : ControllerBase
{


    [HttpGet]
    [Route("GetAllLegislativeSessions")]
    public async Task<ActionResult> GetAllLegislativeSessions()
    {
        try
        {
            var response = await _unitofwork.LegislativeSessions.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {nameof(GetAllLegislativeSessions)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAllLegislativeSessionsByIdYear/{Idyear:guid}")]
    public async Task<ActionResult> GetAllLegislativeSessionsByIdYear(Guid Idyear)
    {
        try
        {
            var response = await _unitofwork.LegislativeSessions.getWithOptions(s => s.IdYear == Idyear);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {nameof(GetAllLegislativeSessions)} pleas view log file");
        }
    }
}

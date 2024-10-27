namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LegislativeYearsController(IUnitOfWork _unitofwork, ILogger<LegislativeYearsController> _logger) : ControllerBase
{

    [HttpGet]
    [Route("GetAllLegislativeYears")]
    public async Task<ActionResult> GetAllLegislativeYears()
    {
        try
        {
            var response = await _unitofwork.LegislativeYears.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {nameof(GetAllLegislativeYears)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAllLegislativeYearsByIdLegislative/{LegislativeId:guid}")]
    public async Task<ActionResult> GetAllLegislativeYearsByIdLegislative(Guid LegislativeId)
    {
        try
        {
            var response = await _unitofwork.LegislativeYears.getWithOptions(s => s.LegislativeId == LegislativeId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {nameof(GetAllLegislativeYearsByIdLegislative)} pleas view log file");
        }
    }
}

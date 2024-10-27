namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LegislativesController(IUnitOfWork _unitofwork, ILogger<LegislativesController> _logger) : ControllerBase
{

    [HttpGet]
    [Route("GetAllLegislatives")]
    public async Task<ActionResult> GetAllLegislativesAsync()
    {
        try
        {
            var response = await _unitofwork.Legislatives.GetAllAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {nameof(GetAllLegislativesAsync)} pleas view log file");
        }
    }
}

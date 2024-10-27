namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhasesController(IUnitOfWork _unitOfWork, ILogger<PhasesController> _logger) : ControllerBase
{

    [HttpGet]
    [Route("getAllPhase")]
    public async Task<IActionResult> getAll()
    {
        try
        {
            var result = await _unitOfWork.Phases.getWithOptions(p => p.IsDeleted == false);
            return Ok(result.OrderBy(p => p.Order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("Error on getAll phases pleas view log file");
        }
    }

    [HttpGet]
    [Route("getPhaseByOrder/{Order:int}")]
    public async Task<IActionResult> getPhaseByOrder(int Order)
    {
        try
        {
            var result = await _unitOfWork.Phases.findAsync(p => p.Order == Order && p.IsDeleted == false);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("Error on getPhaseByOrder pleas view log file");
        }
    }


    [HttpGet]
    [Route("getPhaseById/{Id:guid}")]
    public async Task<IActionResult> getPhaseById(Guid Id)
    {
        try
        {
            var result = await _unitOfWork.Phases.GetByIdAsync(Id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("Error on getPhaseById pleas view log file");
        }
    }
}

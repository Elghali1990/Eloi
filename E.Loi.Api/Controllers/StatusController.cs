namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusController(IUnitOfWork _unitofwork, ILogger<StatusController> _logger) : ControllerBase
{
    [HttpGet]
    [Route("getStatus")]
    public async Task<IActionResult> getStatus()
    {
        try
        {
            var status = await _unitofwork.Status.GetAllStatusWithLaws();
            return Ok(status.OrderBy(s => s.Order));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getStatus)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getStatusByOrder/{order:int}")]
    public async Task<IActionResult> getStatusByOrder(int order)
    {
        try
        {
            var statu = await _unitofwork.Status.getByOrder(order);
            return Ok(statu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getStatus)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getStatuByPhaseId/{phaseId:guid}/{Order:int}")]
    public async Task<IActionResult> getStatuByPhaseId(Guid phaseId, int Order)
    {
        try
        {
            var statu = await _unitofwork.Status.getWithOptions(s => s.Order == Order && s.PhaseId == phaseId);
            return Ok(statu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getStatuByPhaseId)} pleas view log file");
        }
    }



    [HttpGet]
    [Route("getStatusById/{Id:guid}")]
    public async Task<ActionResult> getStatusById(Guid Id)
    {
        try
        {
            var statu = await _unitofwork.Status.GetByIdAsync(Id);
            return Ok(statu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getStatusById)} pleas view log file");
        }
    }
}

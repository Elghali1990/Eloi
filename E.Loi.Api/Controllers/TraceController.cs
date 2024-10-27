using E.Loi.DataAccess.Models;

namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TraceController(ILogger<TraceController> _logger, IUnitOfWork _uof) : ControllerBase
{
    [HttpPost("insertTrace")]
    public async Task<ActionResult> insertTrace([FromBody] Trace trace)
    {
        try
        {
            var result = await _uof.Trace.CreateAsync(trace);
            return Ok(new
            {
                flag = result,
                statu = result ? "success" : "fail"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on ${insertTrace} pleas view log file");
        }
    }
}

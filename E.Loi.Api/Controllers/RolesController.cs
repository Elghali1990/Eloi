namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IUnitOfWork _uof, ILogger<RolesController> _logger) : ControllerBase
{
    [HttpGet]
    [Route("getAllAsync")]
    public async Task<ActionResult> getAllAsync()
    {
        try
        {
            var roles = await _uof.Roles.GetAllAsync();
            return Ok(roles);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error On {nameof(getAllAsync)} Pleas view Log File");
        }
    }
}

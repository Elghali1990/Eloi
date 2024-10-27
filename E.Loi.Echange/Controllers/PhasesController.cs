namespace E.Loi.Echange.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PhasesController(IUnitOfWork _uof) : ControllerBase
{
    [HttpGet]
    [Route("getAllAsync")]
    public async Task<IActionResult> getAllAsync()
    {
        try
        {
            var phases = await _uof.Phases.getWithOptions(p => p.IsDeleted == false);
            return Ok(phases.OrderBy(p => p.Order).Select(p => new PhaseDTOs() { Id = p.Id, Label = p.Title, Order = p.Order }));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

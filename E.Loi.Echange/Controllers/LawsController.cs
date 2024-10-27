namespace E.Loi.Echange;
[Route("api/[controller]")]
[ApiController]
public class LawsController(IUnitOfWork _uof, ILogger<LawsController> _logger) : ControllerBase
{
    [HttpPost]
    [Route("createLawAsync")]
    public async Task<IActionResult> createLawAsync([FromBody] LawDto lawDto)
    {
        try
        {
            var response = await _uof.Laws.CreateLawAsync(lawDto: lawDto);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(createLawAsync)}", nameof(LawsController));
            return BadRequest("Error when create law");
        }
    }
}

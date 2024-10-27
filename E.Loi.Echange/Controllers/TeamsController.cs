namespace E.Loi.Echange.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamsController(IUnitOfWork _uof, ILogger<TeamsController> _logger) : ControllerBase
{
    [HttpPost]
    [Route("getAllAsync/{LawId:guid}")]
    public async Task<IActionResult> getAllAsync([FromBody] List<Guid> Ids, Guid LawId)
    {
        try
        {
            try
            {
                var teamDtos = await _uof.Teams.GetSelecteTeamsForEchange(Ids, LawId);
                return Ok(teamDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error when calling {nameof(getAllAsync)}", nameof(TeamsController));
                return BadRequest(ex.Message);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error when calling {nameof(getAllAsync)}", nameof(TeamsController));
            return BadRequest(ex.Message);
        }
    }
}

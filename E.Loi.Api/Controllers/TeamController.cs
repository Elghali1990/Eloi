namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamController(IUnitOfWork _unitofwork, ILogger<TeamController> _logger) : ControllerBase
{

    [HttpGet]
    [Route("getAllTeams")]
    public async Task<IActionResult> getAllTeams()
    {
        try
        {
            var teams = await _unitofwork.Teams.GetTeamsAllAsync();
            return Ok(teams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {getAllTeams} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getAllTeamsForEchange")]
    public async Task<IActionResult> getAllTeamsForEchange()
    {
        try
        {
            var teams = await _unitofwork.Teams.GetAllTeamsForEchange();
            return Ok(teams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {getAllTeamsForEchange} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var teams = await _unitofwork.Teams.GetAll();
            return Ok(teams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetAll} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getCommissionsAsync")]
    public async Task<IActionResult> GetCommissionsAsync()
    {
        try
        {
            var teams = await _unitofwork.Teams.GetCommissionsAsync();
            return Ok(teams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetCommissionsAsync} pleas view log file");
        }
    }

    [HttpPost]
    [Route("CreateTeamAsync/{CreatedBy:guid}")]
    public async Task<ActionResult> CreateTeamAsync([FromBody] TeamVm vm, Guid CreatedBy)
    {
        try
        {
            var response = await _unitofwork.Teams.CreateTeamAsync(vm, CreatedBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {CreateTeamAsync} pleas view log file");
        }
    }

    [HttpPut]
    [Route("UpdateTeamAsync/{LastModifiedBy:guid}")]
    public async Task<ActionResult> UpdateTeamAsync([FromBody] TeamVm vm, Guid LastModifiedBy)
    {
        try
        {
            var response = await _unitofwork.Teams.UpdateTeamAsync(vm, LastModifiedBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {UpdateTeamAsync} pleas view log file");
        }
    }

    [HttpDelete]
    [Route("DelteTeamAsync/{Id:guid}/{LastModifiedBy:guid}")]
    public async Task<ActionResult> DelteTeamAsync(Guid Id, Guid LastModifiedBy)
    {
        try
        {
            var response = await _unitofwork.Teams.DelteTeamAsync(Id, LastModifiedBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {DelteTeamAsync} pleas view log file");
        }
    }

}

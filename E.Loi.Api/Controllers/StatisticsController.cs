namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatisticsController(IUnitOfWork _uof, ILogger<StatisticsController> _logger) : ControllerBase
{

    [HttpGet]
    [Route("statisticsByCommittees")]
    public async Task<IActionResult> statisticsByCommittees()
    {
        try
        {
            var result = await _uof.Statistics.StatisticsByCommittees();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {statisticsByCommittees}  pleas view log file");
        }
    }

    [HttpGet]
    [Route("statisticsByParliamentaryTeams")]
    public async Task<IActionResult> statisticsByParliamentaryTeams()
    {
        try
        {
            var result = await _uof.Statistics.StatisticsByParliamentaryTeams();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {statisticsByParliamentaryTeams}  pleas view log file");
        }
    }

    [HttpGet]
    [Route("getStatisticReadOne")]
    public async Task<IActionResult> getStatisticReadOne()
    {
        try
        {
            var response = await _uof.Statistics.StatisticsReadOne();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {getStatisticReadOne}  pleas view log file");
        }
    }

    [HttpGet]
    [Route("getStatisticReadTwo")]
    public async Task<IActionResult> getStatisticReadTwo()
    {
        try
        {
            var response = await _uof.Statistics.StatisticsReadTwo();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {getStatisticReadTwo}  pleas view log file");
        }
    }

    [HttpPost]
    [Route("filterStatisticReadOne")]
    public async Task<IActionResult> filterStatisticReadOne([FromBody] SearchDtos search)
    {
        try
        {
            var response = await _uof.Statistics.StatisticsReadOne(search);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {filterStatisticReadOne}  pleas view log file");
        }
    }

    [HttpPost]
    [Route("filterStatisticReadTwo")]
    public async Task<IActionResult> filterStatisticReadTwo([FromBody] SearchDtos search)
    {
        try
        {
            var response = await _uof.Statistics.StatisticsReadTwo(search);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error {filterStatisticReadTwo}  pleas view log file");
        }
    }
}

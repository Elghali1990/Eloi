namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VoteNodesController(IUnitOfWork _unitofwork, ILogger<VoteNodesController> _logger) : ControllerBase
{
    [HttpPost]
    [Route("InsertVote")]
    public async Task<ActionResult> InsertVote([FromBody] VoteVm vote)
    {
        try
        {
            var response = await _unitofwork.VoteNodes.InsertVoteAsync(vote);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("Error on InsertVote pleas view log file");
        }
    }

    [HttpPost]
    [Route("DeleteVote")]
    public async Task<ActionResult> DeleteVote([FromBody] DeleteVoteVm vote)
    {
        try
        {
            var response = await _unitofwork.VoteNodes.DeleteVoteAsync(vote);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("Error on InsertVote pleas view log file");
        }
    }

    [HttpPut]
    [Route("UpdateVote")]
    public async Task<ActionResult> UpdateVote([FromBody] VoteVm vote)
    {
        try
        {
            var response = await _unitofwork.VoteNodes.UpdateVoteAsync(vote);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("Error on UpdateVote pleas view log file");
        }
    }
}

namespace E.Loi.Echange.Controllers;
[Route("api/[controller]")]
[ApiController]
public class VoteController(IUnitOfWork _uof, ILogger<VoteController> _logger, IHttpClientFactory _httpClientFactory) : ControllerBase
{
    HttpClient _httpClient = _httpClientFactory.CreateClient("BaseUrlFinance");

    [HttpGet]
    [Route("getVoteNodesAsync/{sectionId:guid}")]
    public async Task<IActionResult> GetVoteNodesAsync(Guid sectionId)
    {
        try
        {
            //var voteNodes = (await _uof.Nodes.GetFlatNodes(null!, LawId, PhaseLawId))
            var voteNodes = (await _uof.Nodes.GetSectionWithNodes(sectionId, null))

                            .Select(v => new VoteDto
                            {
                                id = v.VoteId,
                                entityId = v.IdFinance ?? Guid.Empty,
                                inFavor = v.InFavor,
                                against = v.Against,
                                neutral = v.Neutral,
                                suppressed = v.suppressed,
                                observation = v.Observation == null ? string.Empty : v.Observation,
                                result = v.ResultFr,
                            }).ToList();
            var section = await _uof.Nodes.GetByIdAsync(sectionId);
            if (section.Number == 2)
            {

                var parentNode = await _uof.Nodes.GetNodeByID(section.ParentNodeId ?? Guid.Empty);
                var voteDto = new VoteDto
                {
                    id = parentNode.VoteNodeResult.Id,
                    entityId = parentNode.IdFinance ?? Guid.Empty,
                    inFavor = parentNode.VoteNodeResult.InFavor,
                    against = parentNode.VoteNodeResult.Against,
                    neutral = parentNode.VoteNodeResult.Neutral,
                    suppressed = parentNode.VoteNodeResult.Suppressed,
                    observation = parentNode.VoteNodeResult.Observation == null ? string.Empty : parentNode.VoteNodeResult.Observation,
                    result = parentNode.VoteNodeResult.Result,
                };
                voteNodes.Add(voteDto);
            }

            return Ok(voteNodes);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, $"Error when colling {nameof(GetVoteNodesAsync)}", nameof(VoteController));
            return BadRequest(e.Message);
        }
    }

    [HttpGet]
    [Route("getVoteAmendmentsAsync/{NodeId:guid}")]
    public async Task<IActionResult> GetVoteAmendmentsAsync(Guid NodeId)
    {
        try
        {
            var voteAmendments = (await _uof.Amendmnets.GetVoteAmendmentsAsync(NodeId))
                                     .Select(v => new VoteDto
                                     {
                                         id = v.Id,
                                         entityId = v.IdFinance ?? Guid.Empty,
                                         inFavor = v.Infavor,
                                         against = v.Against,
                                         neutral = v.Neutral,
                                         suppressed = v.suppressed,
                                         observation = v.Observation == null ? string.Empty : v.Observation,
                                         result = v.Result,
                                     }).ToList(); ;
            return Ok(voteAmendments);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, $"Error when colling {nameof(GetVoteAmendmentsAsync)}", nameof(VoteController));
            return BadRequest(e.Message);
        }
    }
}

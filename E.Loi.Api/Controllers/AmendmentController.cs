namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AmendmentController(IUnitOfWork _unitofwork, ILogger<AmendmentController> _logger, HttpClient http, IConfiguration _config) : ControllerBase
{


    [HttpGet]
    [Route("GetAmendmentsForCluster/{nodeId:guid}")]
    public async Task<IActionResult> GetAmendmentsForCluster(Guid nodeId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetAmendmentsForCluster(nodeId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on GetAmendmentsForCluster pleas view log file");
        }
    }

    [HttpGet]
    [Route("countAmendmentsByTeamAndLaw/{LawId:guid}/{PhaseLawId:guid}")]
    public async Task<IActionResult> CountAmendmentsByTeamAndLaw(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var laws = await _unitofwork.Amendmnets.CountAmendmentsByTeamAndLaw(LawId, PhaseLawId);
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(CountAmendmentsByTeamAndLaw)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("checkBeforChangeAmendmentNode/{NodeId:guid}/{AmendmentId:guid}")]
    public async Task<IActionResult> CheckBeforChangeAmendmentNode(Guid NodeId, Guid AmendmentId)
    {
        try
        {
            var laws = await _unitofwork.Amendmnets.CheckBeforChangeAmendmentNode(NodeId, AmendmentId);
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(CheckBeforChangeAmendmentNode)} pleas view log file");
        }
    }
    

    [HttpGet]
    [Route("GetAmendmentsForCluster/{LawId:guid}/{PhaseLawId:guid}")]
    public async Task<IActionResult> GetAmendmentsForCluster(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetAmendmentsForCluster(LawId, PhaseLawId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on GetAmendmentsForCluster pleas view log file");
        }
    }


    [HttpGet]
    [Route("GetSubmitedAmendmentsListAsync/{nodeId:guid}")]
    public async Task<IActionResult> GetSubmitedAmendmentsListAsync(Guid nodeId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetSubmitedAmendmentsListAsync(nodeId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetSubmitedAmendmentsListAsync} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetNodeAmendmentsAsync/{nodeId:guid}")]
    public async Task<IActionResult> GetNodeAmendmentsAsync(Guid nodeId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetNodeAmendmentsAsync(nodeId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetNodeAmendmentsAsync} pleas view log file");
        }
    }

    [HttpGet]
    [Route("ReassignmentAmendment/{AmendmentId:guid}/{NodeId:guid}/{LastModifiedBy:guid}")]
    public async Task<IActionResult> ReassignmentAmendment(Guid AmendmentId, Guid NodeId, Guid LastModifiedBy)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.ReassignmentAmendment(AmendmentId: AmendmentId, NodeId: NodeId, LastModifiedBy: LastModifiedBy);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {ReassignmentAmendment} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAmendmentByIdAsync/{amendmentId:guid}")]
    public async Task<IActionResult> GetAmendmentByIdAsync(Guid amendmentId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetAmendmentByIdAsync(amendmentId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetAmendmentByIdAsync} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAmendmentsPublicAsync/{nodeId:guid}")]
    public async Task<IActionResult> GetAmendmentsPublicAsync(Guid nodeId)
    {
        try
        {
            List<AmendmentsListVm> amendments = new();
            var result = await _unitofwork.Amendmnets.GetAmendmentsPublicAsync(nodeId);
            foreach (var vm in result)
            {
                var clusters = await _unitofwork.Amendmnets.GetClusterAmendments(vm.Id);
                if (clusters.Count() == 0) amendments.Add(vm);
            }
            return Ok(amendments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetAmendmentsPublicAsync} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAmendmentsListForVotingAsync/{nodeId:guid}")]
    public async Task<IActionResult> GetAmendmentsListForVotingAsync(Guid nodeId)
    {
        try
        {
            List<AmendmentsListVm> amendments = new();
            var result = await _unitofwork.Amendmnets.GetAmendmentsListForVotingAsync(nodeId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetAmendmentsListForVotingAsync} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetPublicAmendmentsListAsync/{nodeId:guid}")]
    public async Task<IActionResult> GetPublicAmendmentsListAsync(Guid nodeId)
    {
        try
        {
            List<AmendmentsListVm> amendments = new();
            var result = await _unitofwork.Amendmnets.GetPublicAmendmentsListAsync(nodeId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetPublicAmendmentsListAsync} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetClusterAmendments/{AmendmentId:guid}")]
    public async Task<IActionResult> GetClusterAmendments(Guid AmendmentId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetClusterAmendments(AmendmentId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetClusterAmendments} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAmendmentDetailsAsync/{AmendmentId:guid}")]
    public async Task<IActionResult> GetAmendmentDetailsAsync(Guid AmendmentId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetAmendmentDetailsAsync(AmendmentId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetAmendmentDetailsAsync} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAllSubmitedAmendments/{nodeId}")]
    public async Task<ActionResult> GetAllSubmitedAmendments(Guid NodeId)
    {
        try
        {
            var response = await _unitofwork.Amendmnets.GetAllSubmitedAmendments(NodeId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest("Error on GetAllSubmitedAmendments pleas view log file");
        }
    }


    [HttpGet]
    [Route("getAmendmentsAsync/{LawId:guid}/{PhaseLawId:guid}")]
    public async Task<ActionResult> GetAmendmentsAsync(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            //  var response = await _unitofwork.Amendmnets.GetAmendmentsAsync(LawId, PhaseLawId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error when caling GetAmendmentsAsync pleas view log file");
        }
    }

    [HttpGet]
    [Route("getVoteAmendmentsAsync/{NodeId:guid}")]
    public async Task<ActionResult> GetVoteAmendmentsAsync(Guid NodeId)
    {
        try
        {
            var response = await _unitofwork.Amendmnets.GetVoteAmendmentsAsync(NodeId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error when caling GetVoteAmendmentsAsync pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAllSubmitedAmendments/{LawId:guid}/{PhaseLawId:guid}")]
    public async Task<ActionResult> GetAllSubmitedAmendments(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _unitofwork.Amendmnets.GetAllSubmitedAmendments(LawId, PhaseLawId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on GetAllSubmitedAmendments pleas view log file");
        }
    }

    [HttpGet]
    [Route("CheckAmendmentsHasNewContent/{LawId:guid}/{PhaseLawId:guid}")]
    public async Task<ActionResult> CheckAmendmentsHasNewContent(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var response = await _unitofwork.Amendmnets.CheckAmendmentsHasNewContent(LawId, PhaseLawId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {CheckAmendmentsHasNewContent} pleas view log file");
        }
    }

    [HttpGet]
    [Route("checkAmendmentsSectionHasNewContent/{NodeId:guid}")]
    public async Task<ActionResult> CheckAmendmentsSectionHasNewContent(Guid NodeId)
    {
        try
        {
            var response = await _unitofwork.Amendmnets.CheckAmendmentsSectionHasNewContent(NodeId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {CheckAmendmentsSectionHasNewContent} pleas view log file");
        }
    }


    [HttpGet]
    [Route("insertGovernmentAmendmentsAsync/{lawId:guid}/{phasLawId:guid}/{TeamId:guid}")]
    public async Task<IActionResult> insertGovernmentAmendmentsAsync(Guid lawId, Guid phasLawId, Guid TeamId)
    {
        var response = await http.GetAsync($"{_config["BaseUrlEchange"]!}DataExchange/getGovernmentAmendmentsAsync/{lawId}/{phasLawId}/{TeamId}");
        if (response.IsSuccessStatusCode)
        {
            var amendmentsDto = (await response.Content.ReadFromJsonAsync<List<AmendmentDto>>())!;
            var result = await _unitofwork.Amendmnets.insertGovernmentAmendmentsAsync(amendmentsDto);
            return Ok(result);
        }
        return Ok();
    }

    [HttpGet]
    [Route("statistic/{lawId:guid}/{phasLawId:guid}")]
    public async Task<IActionResult> Statistic(Guid lawId, Guid phasLawId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.Statistic(lawId, phasLawId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {Statistic} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getVoteResult/{lawId:guid}/{phasLawId:guid}")]
    public async Task<IActionResult> getVoteResult(Guid lawId, Guid phasLawId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetVoteResult(lawId, phasLawId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {Statistic} pleas view log file");
        }
    }

    [HttpPost]
    [Route("createAmendment")]
    public async Task<IActionResult> CreateAmendmentAsync([FromBody] AmendmentVm amendment)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.CreateAmendmantAsync(amendment);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {CreateAmendmentAsync} pleas view log file");
        }
    }

    [HttpPost]
    [Route("CloneAmendmentsAsync")]
    public async Task<IActionResult> CloneAmendmentsAsync([FromBody] CloneAmendmentsVm amendments)
    {
        try
        {
            var response = await _unitofwork.Amendmnets.CloneAmendmentsAsync(amendments);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {CloneAmendmentsAsync} pleas view log file");
        }
    }


    [HttpPost]
    [Route("changeAmendmentTeam/{TeamId:guid}/{LastModifiedBy:guid}")]
    public async Task<IActionResult> ChangeAmendmentTeam([FromBody] List<Guid> AmendmentIds, Guid TeamId, Guid LastModifiedBy)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.ChangeAmendmentTeam(AmendmentIds, TeamId, LastModifiedBy);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {ChangeAmendmentTeam} pleas view log file");
        }
    }



    [HttpPost]
    [Route("SetNewContent")]
    public async Task<IActionResult> SetNewContent([FromBody] SetContent model)
    {
        try
        {
            var response = await _unitofwork.Amendmnets.SetNewContent(model);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {SetNewContent} pleas view log file");
        }
    }

    [HttpPost]
    [Route("GetLiftAmendments/{LawId:guid}/{PhaseId:guid}")]
    public async Task<IActionResult> GetLiftAmendments(List<Guid> TeamIds, Guid LawId, Guid PhaseId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetLiftAmendments(TeamIds, LawId, PhaseId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetLiftAmendments} pleas view log file");
        }
    }

    [HttpPost]
    [Route("checkAmendmentExisteByNumberAsync/{LawId:guid}/{phaseId:guid}/{Number:int}")]
    public async Task<IActionResult> CheckAmendmentExisteByNumberAsync(List<Guid> TeamIds, Guid LawId, Guid phaseId, int Number)
    {

        try
        {
            var response = await _unitofwork.Amendmnets.CheckAmendmentExisteByNumberAsync(LawId, phaseId, TeamIds, Number);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {CheckAmendmentExisteByNumberAsync} pleas view log file");
        }
    }

    [HttpPut]
    [Route("UpdateAmendmantAsync/{amendmentId:guid}")]
    public async Task<IActionResult> UpdateAmendmantAsync(Guid amendmentId, AmendmentVm amendment)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.UpdateAmendmantAsync(amendmentId, amendment);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {UpdateAmendmantAsync} pleas view log file");
        }
    }

    [HttpPost]
    [Route("GetAmendmentsListAsync/{nodeId:guid}")]
    public async Task<IActionResult> GetAmendmentsListAsync(List<Guid> teamIds, Guid nodeId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.GetAmendmentsListAsync(teamIds, nodeId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetAmendmentsListAsync} pleas view log file");
        }
    }
    [HttpPost]
    [Route("SetAmendmentsOrders")]
    public async Task<IActionResult> SetAmendmentsOrders([FromBody] SetAmendmentOrder model)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.SetAmendmentsOrders(model);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {SetAmendmentsOrders} pleas view log file");
        }
    }

    [HttpPost]
    [Route("SetAmendmentsNumbers/{LawId:guid}/{PhaseLawId:guid}/{LastModifiedBy:guid}")]
    public async Task<IActionResult> SetAmendmentsNumbers([FromBody] List<Guid> TeamsId, Guid LawId, Guid PhaseLawId, Guid LastModifiedBy)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.SetAmendmentsNumbers(TeamIds: TeamsId, LawId: LawId, PhaseLawId: PhaseLawId, LastModifiedBy: LastModifiedBy);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {SetAmendmentsNumbers} pleas view log file");
        }
    }

    [HttpPut]
    [Route("SetAmendmentsAsync")]
    public async Task<IActionResult> SetAmendmentsAsync(SetAmendmentStatuVm amendmentStatuVm)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.SetAmendmentsAsync(amendmentStatuVm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {SetAmendmentsAsync} pleas view log file");
        }
    }

    [HttpDelete]
    [Route("DeleteAmendmantAsync/{amendmentId:guid}/{userId:guid}")]
    public async Task<IActionResult> DeleteAmendmantAsync(Guid amendmentId, Guid userId)
    {
        try
        {
            var result = await _unitofwork.Amendmnets.DeleteAmendmantAsync(amendmentId, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {DeleteAmendmantAsync} pleas view log file");
        }
    }

}

namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NodesController(IUnitOfWork _unitofwork, ILogger<NodesController> _logger, HttpClient http, IConfiguration _config) : ControllerBase
{

    [HttpGet]
    [Route("getNodeChildrens")]
    public async Task<ActionResult> getNodeChildrens(string lawId, string phaseId, bool includeVirtuelNode)
    {
        try
        {
            Guid LawId = Guid.Parse(lawId);
            Guid PhaseId = Guid.Parse(phaseId);
            var data = await _unitofwork.Nodes.GetNodes(LawId, PhaseId, includeVirtuelNode);

            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getNodeChildrens)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetFlatNodes/{lawId:guid}/{phaseId:guid}")]
    public async Task<ActionResult> GetFlatNodes(Guid lawId, Guid phaseId)
    {
        try
        {
            var data = await _unitofwork.Nodes.GetFlatNodes(null!, lawId, phaseId);
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetFlatNodes} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getNodeByID/{NodeId:guid}")]
    public async Task<ActionResult> GetNodeByID(Guid NodeId)
    {
        try
        {
            var node = await _unitofwork.Nodes.GetNodeByID(NodeId);
            return Ok(node);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetNodeByID} pleas view log file");
        }
    }


    [HttpGet]
    [Route("getNodeByTypeByNumberByBis/{ParentNodeId:guid}/{TypeId:guid}/{Number:int}/{Bis:int}")]
    public async Task<ActionResult> GetNodeByTypeByNumberByBis(Guid ParentNodeId,Guid TypeId, int Number, int Bis)
    {
        try
        {
            var response = await _unitofwork.Nodes.GetNodeByTypeByNumberByBis(ParentNodeId,TypeId, Number,Bis);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetNodeByID} pleas view log file");
        }
    }

   

    [HttpGet]
    [Route("getLawSections/{lawId:guid}/{phaseId:guid}/{IsPrinVotingFile:bool}")]
    public async Task<ActionResult> getLawSections(Guid lawId, Guid phaseId, bool IsPrinVotingFile)
    {
        try
        {
            var data = await _unitofwork.Nodes.GetLawSections(lawId, phaseId, IsPrinVotingFile);
            return Ok(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {getLawSections} pleas view log file");
        }
    }


    [HttpGet]
    [Route("getNodeContent/{NodeId}")]
    public async Task<ActionResult> getNodeContent(Guid NodeId)
    {
        try
        {
            var nodeContent = await _unitofwork.Nodes.GetNodeContent(NodeId);
            return Ok(nodeContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {getNodeContent} pleas view log file");
        }
    }

    [HttpGet]
    [Route("CloneNodes/{LawId:guid}/{CurentPhase:guid}/{DestinationPhase:guid}/{Statu}")]
    public async Task<ActionResult> CloneNodes(Guid LawId, Guid CurentPhase, Guid DestinationPhase, string Statu)
    {
        try
        {
            var response = await _unitofwork.Nodes.CloneNodes(LawId, CurentPhase, DestinationPhase, Statu);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {CloneNodes} pleas view log file");
        }
    }


    [HttpGet]
    [Route("cloneSectionWithChildrens/{SectionId:guid}/{DestinationPhase:guid}")]
    public async Task<ActionResult> cloneSectionWithChildrens(Guid SectionId, Guid DestinationPhase)
    {
        try
        {
            var response = await _unitofwork.Nodes.cloneSectionWithChildrens(SectionId, DestinationPhase);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {cloneSectionWithChildrens} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetFlatParents/{NodeId:guid}")]
    public async Task<ActionResult> GetFlatParents(Guid NodeId)
    {
        try
        {
            var response = await _unitofwork.Nodes.GetFlatParents(NodeId, null!);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {GetFlatParents} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetNodeByIdAsync/{Id:guid}")]
    public async Task<ActionResult> GetNodeByIdAsync(Guid Id)
    {
        try
        {
            var response = await _unitofwork.Nodes.GetByIdAsync(Id);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetNodeByIdAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetParentNode/{LawId:guid}/{PhaseLawId:guid}")]
    public async Task<ActionResult> GetParentNode(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var node = await _unitofwork.Nodes.GetParentNode(lawId: LawId, phaseLawId: PhaseLawId);
            return Ok(node);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetParentNode)} pleas view log file");
        }
    }


    [HttpGet]
    [Route("GetDirecteChilds/{ParentId:guid}")]
    public async Task<ActionResult> GetDirecteChildsAsync(Guid ParentId)
    {
        try
        {
            var response = await _unitofwork.Nodes.GetDirecteChilds(ParentId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetDirecteChildsAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("SetPhaseNodes/{LawId:guid}/{DestinationPhase:guid}/{LastModifyBy:guid}/{Order:int}")]
    public async Task<ActionResult> SetPhaseNodes(Guid LawId, Guid DestinationPhase, Guid LastModifyBy, int Order)
    {
        try
        {
            var response = await _unitofwork.Nodes.SetPhaseNodes(LawId, DestinationPhase, LastModifyBy, Order);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(SetPhaseNodes)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetFlatPrint")]
    public async Task<ActionResult> GetFlatPrint(Guid LawId, Guid PhasLawId)
    {
        try
        {
            var response = await _unitofwork.Nodes.GetFlatPrint(LawId, PhasLawId, null!);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(SetPhaseNodes)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getSectionRubric/{SectionId:guid}")]
    public async Task<ActionResult> GetSectionRubric(Guid SectionId)
    {
        try
        {
            var response = await _unitofwork.Nodes.GetSectionRubric(SectionId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetSectionRubric)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getNodeWithAmendmentForPresident")]
    public async Task<ActionResult> getNodeWithAmendmentForPresident(Guid LawId, Guid PhasLawId)
    {
        try
        {
            var response = await _unitofwork.Nodes.GetNodeWithAmendmentForPresident(LawId, PhasLawId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(SetPhaseNodes)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetSectionForVoting")]
    public async Task<ActionResult> GetSectionForVoting(Guid nodeId)
    {
        try
        {
            var response = await _unitofwork.Nodes.GetSectionForVoting(nodeId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetSectionForVoting)} pleas view log file");
        }
    }

    


    [HttpPost]
    [Route("CreateVirtualNode")]
    public async Task<ActionResult> CreateVirtualNode([FromBody] CreateNodeVm model)
    {
        try
        {
            var response = await _unitofwork.Nodes.CreateVirtuelNode(model);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(CreateVirtualNode)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("setNewContent")]
    public async Task<IActionResult> SetNewContent([FromBody] UpdateNodeContent updateNodeContent)
    {
        try
        {
            var response = await _unitofwork.Nodes.SetNewContent(updateNodeContent);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(SetNewContent)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("CreateLawNodes/{LawId:guid}/{CreatedBy}")]
    public async Task<ActionResult> CreatelawNodes([FromBody] List<TextLaw> texts, Guid LawId, Guid CreatedBy)
    {

        try
        {
            var response = await _unitofwork.Nodes.CreateLawNodes(texts, LawId, CreatedBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(CreatelawNodes)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("CreateNode")]
    public async Task<ActionResult> CreateNode([FromBody] CreateNodeVm model)
    {
        try
        {
            var response = await _unitofwork.Nodes.CreateNode(model);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(CreateNode)} pleas view log file");
        }
    }

    [HttpPut]
    [Route("UpdateNodeContent/{UserId:guid}")]
    public async Task<ActionResult> UpdateNodeContent([FromBody] NodeContentVm nodeContent, Guid UserId)
    {
        try
        {
            var response = await _unitofwork.Nodes.UpdateNodeContent(nodeContent, UserId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(UpdateNodeContent)} pleas view log file");
        }
    }

    [HttpDelete]
    [Route("DeleteNode/{nodeId:guid}/{userId:guid}")]
    public async Task<ActionResult> DeleteNode(Guid nodeId, Guid userId)
    {
        try
        {
            var response = await _unitofwork.Nodes.DeleteNode(nodeId, userId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(DeleteNode)} pleas view log file");
        }
    }


    [HttpGet]
    [Route("createNodes/{lawId:guid}/{phasLawId:guid}")]
    public async Task<IActionResult> createNodes(Guid lawId, Guid phasLawId)
    {
        var response = await http.GetAsync($"{_config["BaseUrlEchange"]}DataExchange/getNodesAsync/{lawId}/{phasLawId}");
        if (response.IsSuccessStatusCode)
        {
            var nodes = await response.Content.ReadFromJsonAsync<DataAccess.Dtos.NodeDto[]>();
            var result = await _unitofwork.Nodes.CreateNodesLawAsync(nodes!, lawId);
            return Ok(result);
        }
        return Ok();
    }
}

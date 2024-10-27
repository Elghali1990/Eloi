namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NodeTypesController(IUnitOfWork _unitofwork, ILogger<NodeTypesController> _logger) : ControllerBase
{
    [HttpGet]
    [Route("GetAllNodeTypesAsync")]
    public async Task<ActionResult> GetAllNodeTypesAsync()
    {
        try
        {
            var response = await _unitofwork.NodeTypes.getWithOptions(type => type.TextType == "TextLaw");
            // var response =await _unitofwork.NodeTypes.GetNodeTypesAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetAllNodeTypesAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getPlfNodeTypesWithNodeHierarchies")]
    public async Task<ActionResult> GetPlfNodeTypesWithNodeHierarchies()
    {
        try
        {
            var response = await _unitofwork.NodeTypes.GetPlfNodeTypesWithNodeHierarchies();
            return Ok(response);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetPlfNodeTypesWithNodeHierarchies)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getNodeTypesWithNodeHierarchies/{nodeTypeId:guid}")]
    public async Task<ActionResult> GetNodeTypesWithNodeHierarchies(Guid nodeTypeId)
    {
        try
        {
            var response = await _unitofwork.NodeTypes.GetNodeTypesWithNodeHierarchies(nodeTypeId);
            return Ok(response);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetNodeTypesWithNodeHierarchies)} pleas view log file");
        }
    }


    [HttpGet]
    [Route("GetNodeTypeByIdAsync/{nodeTypeId:guid}")]
    public async Task<IActionResult> GetNodeTypeByIdAsync(Guid nodeTypeId)
    {
        try
        {
            var result = await _unitofwork.NodeTypes.GetByIdAsync(nodeTypeId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetNodeTypeByIdAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetNodeTypeByNameAsync/{label}")]
    public async Task<IActionResult> GetNodeTypeByNameAsync(string label)
    {
        try
        {
            var result = await _unitofwork.NodeTypes.findAsync(type => type.Label.Trim() == label.Trim());
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on  {nameof(GetNodeTypeByNameAsync)}  pleas view log filele");
        }
    }
}

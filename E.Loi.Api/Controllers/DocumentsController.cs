namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DocumentsController(IUnitOfWork _uof, ILogger<DocumentsController> _logger) : ControllerBase
{

    [HttpPost]
    [Route("insertDocument")]
    public async Task<IActionResult> InsertDocument([FromBody] List<DocumentVm> documents)
    {
        try
        {
            var response = await _uof.Documents.AddDocumentAsync(documents);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on ${InsertDocument} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetDocumentByIdAsync/{docId}")]
    public async Task<ActionResult> GetDocumentByIdAsync(Guid docId)
    {
        try
        {
            var doc = await _uof.Documents.findAsync(d => d.Id == docId);
            return Ok(doc);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on ${GetDocumentByIdAsync} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetLawDocumentsAsync/{LawId:guid}")]
    public async Task<ActionResult> GetLawDocumentsAsync(Guid LawId)
    {
        try
        {
            var response = await _uof.Documents.GetLawDocumentsAsync(LawId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on ${GetLawDocumentsAsync} pleas view log file");
        }
    }
    [HttpDelete]
    [Route("DeleteDocumentAsync/{DocId:guid}")]
    public async Task<ActionResult> DeleteDocumentAsync(Guid DocId)
    {
        try
        {
            var response = await _uof.Documents.DeleteDocumentAsync(DocId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on ${GetLawDocumentsAsync} pleas view log file");
        }
    }


}

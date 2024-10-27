using E.Loi.Edition.Generation.Generation.TextLaw;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace E.Loi.Edition.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TextLawController(GenerateTextLaw _generateTextLaw, ILogger<TextLawController> _logger) : ControllerBase
{

    [HttpGet]
    [Route("generateTextLaw")]
    public async Task<byte[]> generateTextLaw(Guid LawId, Guid PhaseLawId, string outType)
    {

        try
        {
            FileStream stream = await _generateTextLaw.PrintTextLoi(LawId, PhaseLawId, outType);
            var contentDispositionHeader = new ContentDisposition() { FileName = String.Format("PLF_{0}.{1}", "TextLaw", outType).ToString() };
            Response.Headers.Add("content-disposition", contentDispositionHeader.ToString());
            var ds = new FileStreamResult(stream, "application/pdf");
            var memory = new MemoryStream();
            await ds.FileStream.CopyToAsync(memory);
            ds.FileStream.Close();
            byte[] bytes = memory.ToArray();
            memory.Close();
            return bytes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(generateTextLaw)}", nameof(TextLawController));
            throw;
        }
    }
}

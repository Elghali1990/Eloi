using E.Loi.Edition.Generation.Generation.VotingFile;
using E.Loi.Edition.Generation.VotingFile;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace E.Loi.Edition.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotingFileSessionController(GenerationVotingFile _generationVotingFile,  ILogger<VotingFileSessionController> _logger, GenerateVotingFileForPresident _generateVotingFileForPresident) : ControllerBase
    {
        [HttpGet]
        [Route("generateVotingFile/{LawId:guid}/{sectionId:guid}")]
        public async Task<byte[]> generateVotingFile(Guid LawId, Guid sectionId, string outType)
        {

            try
            {
                FileStream stream = await _generationVotingFile.PrintVotingFileSession(sectionId, LawId, outType);
                var contentDispositionHeader = new ContentDisposition() { FileName = String.Format("PLF_{0}.{1}", "votingfile", outType).ToString() };
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
                _logger.LogError(ex.Message, $"Error on {nameof(generateVotingFile)}", nameof(VotingFileSessionController));
                throw;
            }
        }

        [HttpGet]
        [Route("printVotingFileForPresident/{LawId:guid}/{sectionId:guid}")]
        public async Task<byte[]> printVotingFileForPresident(Guid LawId, Guid sectionId, string outType)
        {

            try
            {
                FileStream stream = await _generateVotingFileForPresident.PrintVotingFileForPresident(LawId, sectionId, outType);
                var contentDispositionHeader = new ContentDisposition() { FileName = String.Format("PLF_{0}.{1}", "votingfile", outType).ToString() };
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
                _logger.LogError(ex.Message, $"Error on {nameof(generateVotingFile)}", nameof(VotingFileSessionController));
                throw;
            }
        }
    }
}

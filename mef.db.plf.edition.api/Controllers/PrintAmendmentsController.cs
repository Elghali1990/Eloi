using E.Loi.DataAccess.Vm.Amendment;
using E.Loi.Edition.Generation.Generation.amendment;
using E.Loi.Edition.Generation.Generation.VotingFile;
using E.Loi.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace E.Loi.Edition.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintAmendmentsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly GenerationAmendments _generationAmendments;
        private readonly GenerateVotingFileCommission _generateVotingFileCommission;
        private readonly GenerateVoteResult _generateVoteResult;

        private ConfImpVoteComission config;
        ILogger<PrintAmendmentsController> _logger;
        public PrintAmendmentsController(IUnitOfWork unitOfWork,
            GenerationAmendments generationAmendments,
            IConfiguration configuration,
            ILogger<PrintAmendmentsController> logger,
            GenerateVotingFileCommission generateVotingFileCommission,
            GenerateVoteResult generateVoteResult)
        {
            _unitOfWork = unitOfWork;
            _generationAmendments = generationAmendments;
            config = new ConfImpVoteComission
            {
                IdParent = Guid.Parse(configuration.GetValue<string>("PRINT:IdParent")!),
                IdPartie = Guid.Parse(configuration.GetValue<string>("PRINT:IdPartie")!),
                IdParagraphe = Guid.Parse(configuration.GetValue<string>("PRINT:IdParagraphe")!),
                IdTitre = Guid.Parse(configuration.GetValue<string>("PRINT:IdTitre")!),
                IdArticle = Guid.Parse(configuration.GetValue<string>("PRINT:IdArticle")!),
                IdAlinea = Guid.Parse(configuration.GetValue<string>("PRINT:IdAlinea")!),
                IdArticleMetier = Guid.Parse(configuration.GetValue<string>("PRINT:IdArticleMetier")!),
                IdTableau = Guid.Parse(configuration.GetValue<string>("PRINT:IdTableau")!),
                IdAnnexe = Guid.Parse(configuration.GetValue<string>("PRINT:IdAnnexe")!),
                IdGroupe = Guid.Parse(configuration.GetValue<string>("PRINT:IdGroupe")!)
            };
            _logger = logger;
            _generateVotingFileCommission = generateVotingFileCommission;
            _generateVoteResult = generateVoteResult;
        }

        [HttpPost(Name = nameof(printTeamAmendments))]
        public async Task<byte[]> printTeamAmendments([FromBody] SetAmendData data)
        {
            try
            {
                FileStream stream = await _generationAmendments.PrintTeamAmendments(data, config);
                var contentDispositionHeader = new ContentDisposition() { FileName = System.String.Format("Suivi_des_amendements." + data.OutType, "").ToString() };
                Response.Headers.Add("content-disposition", contentDispositionHeader.ToString());
                var fs = new FileStreamResult(stream, data.OutType.Equals("pdf") ? "application/pdf" : "application/docx");
                var memory = new MemoryStream();
                await fs.FileStream.CopyToAsync(memory);
                fs.FileStream.Close();
                byte[] bytes = memory.ToArray();
                memory.Close();
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error On {nameof(printTeamAmendments)}", nameof(PrintAmendmentsController));
                throw;
            }
        }



        [HttpGet]
        [Route("printVotingFileCommission")]
        public async Task<byte[]> printVotingFileCommission(Guid LawId, Guid PhaseLawId, string outType, bool includeAmendmentRatraper)
        {
            try
            {
                FileStream stream = await _generateVotingFileCommission.Print(LawId, PhaseLawId, config, outType, includeAmendmentRatraper);
                var contentDispositionHeader = new ContentDisposition() { FileName = System.String.Format("Suivi_des_amendements." + outType, "").ToString() };
                Response.Headers.Add("content-disposition", contentDispositionHeader.ToString());
                var fs = new FileStreamResult(stream, outType.Equals("pdf") ? "application/pdf" : "application/docx");
                var memory = new MemoryStream();
                await fs.FileStream.CopyToAsync(memory);
                fs.FileStream.Close();
                byte[] bytes = memory.ToArray();
                memory.Close();
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error On {nameof(printVotingFileCommission)}", nameof(PrintAmendmentsController));
                throw;
            }
        }

        [HttpGet]
        [Route("printVoteAmendmentsResult")]
        public async Task<byte[]> printVoteAmendmentsResult(Guid LawId, Guid PhaseLawId, string outType)
        {
            try
            {
                FileStream stream = await _generateVoteResult.PrintVoteAmendmentsResult(LawId, PhaseLawId, outType);
                var contentDispositionHeader = new ContentDisposition() { FileName = System.String.Format("Suivi_des_amendements." + outType, "").ToString() };
                Response.Headers.Add("content-disposition", contentDispositionHeader.ToString());
                var fs = new FileStreamResult(stream, outType.Equals("pdf") ? "application/pdf" : "application/docx");
                var memory = new MemoryStream();
                await fs.FileStream.CopyToAsync(memory);
                fs.FileStream.Close();
                byte[] bytes = memory.ToArray();
                memory.Close();
                return bytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, $"Error On {nameof(printVoteAmendmentsResult)}", nameof(PrintAmendmentsController));
                throw;
            }
        }

    }
}

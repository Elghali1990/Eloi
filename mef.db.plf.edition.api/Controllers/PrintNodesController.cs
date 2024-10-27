using E.Loi.DataAccess.Models;
using E.Loi.DataAccess.Vm.Editor;
using E.Loi.Edition.Generation.Generation.Nodes;
using E.Loi.Edition.Generation.Generation.Plf;
using E.Loi.Edition.Generation.VMs;
using E.Loi.Services.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
namespace E.Loi.Edition.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrintNodesController(IUnitOfWork _unitOfWork, GenerateNode _generateNode, GeneratePLF _generatePLF, ILogger<PrintNodesController> _logger) : ControllerBase
{

    [HttpGet]
    [Route("printNode/{NodeId}")]
    public async Task<byte[]> printNode(Guid NodeId)
    {
        try
        {
            FileStream stream = await _generateNode.PrintNode(NodeId);
            var contentDispositionHeader = new ContentDisposition() { FileName = String.Format("PLF_{0}.pdf", "Nodes").ToString() };
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
            _logger.LogError(ex.Message, $"Error On {nameof(printNode)}", nameof(PrintNodesController));
            throw;
        }
    }


    [HttpPost]
    [Route("printEditorContent")]
    public async Task<byte[]> PrintEditorContent(EditorContent editorContent)
    {
        try
        {
            FileStream stream = await _generateNode.PrintEditorContent(editorContent.Content);
            var contentDispositionHeader = new ContentDisposition() { FileName = String.Format("PLF_{0}.pdf", "editorContent").ToString() };
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
            _logger.LogError(ex.Message, $"Error On {nameof(PrintEditorContent)}", nameof(PrintNodesController));
            throw;
        }
    }



    [HttpGet]
    [Route("generate/{LawId:guid}/{PhaseId:guid}")]
    public async Task<ActionResult> Generate(Guid LawId, Guid PhaseId)
    {
        try
        {
            Phase phase_ = await _unitOfWork.Phases.GetByIdAsync(PhaseId);
            var law = await _unitOfWork.Laws.GetByIdAsync(LawId);
            var nodes = await _unitOfWork.Nodes.getWithOptions(n => n.LawId == LawId && n.PhaseLawId == PhaseId && n.ParentNodeId == null);
            var node = nodes.FirstOrDefault();
            var nodes_ = await _unitOfWork.Nodes.GetNodesLawVM(node);
            var nodeTypes = (await _unitOfWork.NodeTypes.GetAllAsync()).Select(t => new E.Loi.Edition.Generation.VMs.NodeType.NodeTypeVM { id = t.Id.ToString(), label = t.Label }).ToArray();
            // HashSet<Guid> get_authorised_nodes = null!;
            SetDocumentData setDocumentData = new()
            {
                name = law.Label,
                format = "pdf",
                nodes = nodes_,
                nodeSelected = nodes_,
                language = "ar",
                phase = new DataAccess.Vm.Phase.PhaseVM() { Id = nameof(phase_.Id), order = phase_.Order, status = string.Empty, lawId = nameof(LawId), Title = phase_.Title },
                law = new DataAccess.Vm.Law.LawVM() { Id = law.Id, Year = law.Year, Label = law.Label, Category = law.Category, Type = law.Type, NodeHierarchyFamillyId = law.NodeHierarchyFamillyId.ToString() }!,
                nodeTypes = nodeTypes,
                printings = true,
                authorised_amendements = null!,
                authorised_nodes = null!,
                typeArticle = string.Empty,
                typePartie = string.Empty,
            };
            FileStream stream = await _generatePLF.Print(setDocumentData);
            string ext = "pdf";

            if (setDocumentData.format != "pdf")
            {
                ext = "docx";
            }
            var contentDispositionHeader = new ContentDisposition() { FileName = String.Format("PLF_{0}_{1}." + ext, setDocumentData.law.Number, setDocumentData.language).ToString() };
            Response.Headers.Add("content-disposition", contentDispositionHeader.ToString());
            return new FileStreamResult(stream, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(Generate)}", nameof(PrintNodesController));
            throw;
        }

    }

}

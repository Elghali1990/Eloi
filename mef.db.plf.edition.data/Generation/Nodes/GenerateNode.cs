using Aspose.Words;
using Aspose.Words.Fonts;
using E.Loi.DataAccess.Vm.Editor;
using E.Loi.Edition.Generation.Helpers;
using E.Loi.Edition.Generation.Services;
using E.Loi.Edition.Generation.VMs.Node;
using E.Loi.Services.Repositories.Interfaces;
using mef.db.plf.edition.generation.Generation;
using mef.db.plf.edition.generation.License;
using System.Web;

namespace E.Loi.Edition.Generation.Generation.Nodes;

public class GenerateNode(IViewRenderService _viewRenderService, IUnitOfWork _unitOfWork)
{


    protected Dictionary<string, string> nodeTypes = new()
    {
        {"Law","قانون" },
        {"Branch","الفرع" },
        {"ArticleMitier","مادة وظيفية" },
        {"Clause","البند" },
        {"Anexxe","الملحق" },
        {"Subject","المادة" },
        {"Title","عنوان" },
        {"Door","الباب" },
        {"Paragraph","الفقرة" },
        {"Section","الجزء" },
        {"ListTarifaireDouanes","جدول التعريفة الجمركية" }
    };

    public async Task<FileStream> PrintNode(Guid NodeId)
    {
        LicenseAspose.setLicenseAsposeTotal("./License/Aspose.Total.NET.lic");
        Document document = new();
        FontSettings.DefaultInstance.SubstitutionSettings.DefaultFontSubstitution.DefaultFontName = "Arial Unicode MS";
        CustomDocumentBuilder builder = new(document);
        ManageEdition.ConfigBuilderLandscape(builder, "ar");
        builder.PageSetup.RestartPageNumbering = true;
        var node_ = await _unitOfWork.Nodes.GetNodeContent(NodeId);
        var node = await _unitOfWork.Nodes.findAsync(n => n.Id == NodeId);
        string Title = await GetNodeLabel(node);
        var html = await _viewRenderService.RenderToStringAsync("node/NodeContent", new NodeTemplate
        {
            NodeId = node.Id,
            Title = Title,
            Content = node_.NodeContent.Replace("text-decoration-line", "text-decoration"),
        });
        var decodedHtml = HttpUtility.HtmlDecode(html);
        builder.InsertHtml(decodedHtml);

        document.UpdatePageLayout();
        var filename = "files/" + String.Format("node.pdf");
        document.Save(filename, SaveFormat.Pdf);
        FileStream stream = new FileStream(filename, FileMode.Open);
        return stream;

        throw new Exception("output type not yet supported");
    }


    public async Task<FileStream> PrintEditorContent(string content)
    {
        LicenseAspose.setLicenseAsposeTotal("./License/Aspose.Total.NET.lic");
        Document document = new();
        FontSettings.DefaultInstance.SubstitutionSettings.DefaultFontSubstitution.DefaultFontName = "Arial Unicode MS";
        CustomDocumentBuilder builder = new(document);
        ManageEdition.ConfigBuilderLandscape(builder, "ar");
        builder.PageSetup.RestartPageNumbering = true;
        var html = await _viewRenderService.RenderToStringAsync("editor-content/EditorContentTemplate", new EditorContent
        {
            Content = content,
        });
        var decodedHtml = HttpUtility.HtmlDecode(html);
        builder.InsertHtml(decodedHtml);

        document.UpdatePageLayout();
        var filename = "files/" + String.Format("editor-content.pdf");
        document.Save(filename, SaveFormat.Pdf);
        FileStream stream = new FileStream(filename, FileMode.Open);
        return stream;

        throw new Exception("output type not yet supported");
    }
    public async Task<string> GetNodeLabel(E.Loi.DataAccess.Models.Node node)
    {
        string repeated = string.Empty;
        string label = string.Empty;
        var nodeType = await _unitOfWork.NodeTypes.findAsync(t => t.Id == node.TypeId);
        if (nodeTypes["Section"].Trim() == nodeType!.Label.Trim())
        {
            label = $"{nodeType.Label} {node.Number} : {node.Label}";
        }
        else if (nodeTypes["Door"].Trim() == nodeType!.Label.Trim())
        {
            label = $"{nodeType.Label} {node.Number} : {node.Label}";
        }
        else if (nodeTypes["Paragraph"].Trim() == nodeType!.Label.Trim())
        {
            label = $"{nodeType.Label} {node.Number} : {node.Label}";
        }
        else if (nodeTypes["Subject"].Trim() == nodeType!.Label.Trim())
        {
            label = $"{nodeType.Label} {node.Number} : {node.Label}";
        }
        else if (nodeTypes["Clause"].Trim() == nodeType!.Label.Trim())
        {
            label = $"{nodeType.Label} {node.Number} : {node.Label}";
        }
        else if (nodeTypes["Title"].Trim() == nodeType!.Label.Trim())
        {
            label = $"{nodeType.Label} {node.Number} : {node.Label}";
        }
        else if (nodeTypes["Anexxe"].Trim() == nodeType!.Label.Trim())
        {
            label = $"{nodeType.Label} {node.Number} : {node.Label}";
        }
        else if (nodeTypes["ListTarifaireDouanes"].Trim() == nodeType!.Label.Trim())
        {
            label = $"{nodeType.Label} {node.Number} : {node.Label}";
        }
        else if (nodeTypes["ArticleMitier"].Trim() == nodeType!.Label.Trim())
        {
            repeated = node.Bis > 0 ? "مكرر " + node.Bis + "" : string.Empty;
            label = $"{nodeType.Label} {node.Number} {repeated} : {node.Label}";
        }
        else
        {
            label = node.Label;
        }
        if (label.Trim().EndsWith(":"))
        {
            label = label.Replace(":", string.Empty);
        }
        return label;
    }


}

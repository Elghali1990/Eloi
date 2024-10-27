using Aspose.Words;
using Aspose.Words.Fonts;
using E.Loi.DataAccess.NodeJsonVm;
using E.Loi.DataAccess.Vm.Node;
using E.Loi.Edition.Generation.Helpers;
using E.Loi.Edition.Generation.Services;
using E.Loi.Services.Repositories.Interfaces;
using mef.db.plf.edition.generation.Generation;
using mef.db.plf.edition.generation.License;
using System.Text;
using System.Web;

namespace E.Loi.Edition.Generation.Generation.TextLaw;

public class GenerateTextLaw(IUnitOfWork _unitOfWork, IViewRenderService _viewRenderService)
{
    public async Task<FileStream> PrintTextLoi(Guid LawId, Guid PhaselawId, string OutType)
    {
        LicenseAspose.setLicenseAsposeTotal("./License/Aspose.Total.NET.lic");
        Document document = new();
        FontSettings.DefaultInstance.SubstitutionSettings.DefaultFontSubstitution.DefaultFontName = "Arial Unicode MS";
        CustomDocumentBuilder builder = new(document);
        ManageEdition.ConfigBuilderLandscape(builder, "ar");
        builder.PageSetup.RestartPageNumbering = true;
        builder.PageSetup.RestartPageNumbering = true;
        var law = await _unitOfWork.Laws.GetByIdAsync(LawId);
        string LawTitle = law.Label;
        var nodes = await _unitOfWork.Nodes.GetFlatPrint(LawId, PhaselawId, null!);
        if (nodes.Count > 0)
        {
            int counter = 0;
            ManageEdition.AddTextLawGuarddPage(builder, LawTitle);
            builder.InsertBreak(BreakType.SectionBreakNewPage);
            foreach (var node in nodes)
            {
                counter++;
                if (counter == 115)
                {
                    string x = "dd";
                }
                await addNode(node, builder);
                //builder.InsertBreak(BreakType.PageBreak);
            }
        }
        for (int i = 0; i < document.Sections.Count; i++)
        {
            document.Sections[i].PageSetup.RestartPageNumbering = true;
            document.Sections[i].PageSetup.PageStartingNumber = 1;
            if (i % 2 == 1)
            {
                ManageEdition.AddHeaderAndFooterAmendment(builder, document, "ar", i);
                document.UpdatePageLayout();
            }
        }

        if (OutType == "pdf")
        {
            var filename = "files/" + System.String.Format(LawTitle + "." + OutType);
            document.Save(filename, SaveFormat.Pdf);
            FileStream stream = new FileStream(filename, FileMode.Open);
            return stream;
        }
        if (OutType == "docx")
        {
            var filename = "files/" + System.String.Format(LawTitle + "." + OutType);
            document.Save(filename, SaveFormat.Docx);
            FileStream stream = new FileStream(filename, FileMode.Open);
            return stream;
        }
        throw new Exception("output type not yet supported");
    }

    private async Task addNode(FlatNode node, CustomDocumentBuilder builder)
    {
        var nodeWithCntent = await _unitOfWork.Nodes.GetNodeContent(node.Id);
        var html = await _viewRenderService.RenderToStringAsync("TextLaw/TextLaw", new FlatNode
        {
            Label = node.Label,
            Content = nodeWithCntent.NodeContent,
            Order = node.Order,
        });
        var decodedHtml = HttpUtility.HtmlDecode(html);
        builder.InsertHtml(decodedHtml);
    }
    public Root ReadJsonString(string stringJson)
    {
        Root root = new();
        root = System.Text.Json.JsonSerializer.Deserialize<Root>(stringJson)!;
        return root;
    }
    public string generateHtmlFromJsonDtat(Root root)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("<style> table, th, td {border: 1px solid #ccc;}</style><table style='border: 1px solid #ccc;'");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th colspan='5' style='max-width:20%;;vertical-align:middle;padding:10px'></th>");
        stringBuilder.Append("<th style='width:50%;vertical-align:middle;text-align:center;padding:10px'>اسم</th>");
        stringBuilder.Append("<th style='width:10%;vertical-align:middle;text-align:center;padding:10px'>الرسم</th>");
        stringBuilder.Append("<th style='width:10%;vertical-align:middle;text-align:center'padding:10px>الوحدة</th>");
        stringBuilder.Append("<th style='width:10%;vertical-align:middle;text-align:center;padding:10px'>الوحدة التكميلية</th>");
        stringBuilder.Append("</tr>");
        foreach (var row in root.rows)
        {
            stringBuilder.Append($"<tr><td style='vertical-align:middle;text-align:center;padding:5px'>{row.nomenclature_level_1}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center;padding:5px'>{row.nomenclature_level_2}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center;padding:5px'>{row.nomenclature_level_3}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center;padding:5px'>{row.nomenclature_level_4}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center;padding:5px'>{row.nomenclature_level_5}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center;padding:5px'>{row.designation}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center;padding:5px'>{row.tarif}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center;padding:5px'>{row.unit}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center;padding:5px'>{row.unit_complementary}</td></tr>");
        }
        stringBuilder.Append("</table>");
        return stringBuilder.ToString();
    }
}



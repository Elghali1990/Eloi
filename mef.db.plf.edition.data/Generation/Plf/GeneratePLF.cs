using Aspose.Words;
using E.Loi.DataAccess.Vm.Law;
using E.Loi.DataAccess.Vm.Node;
using E.Loi.DataAccess.Vm.Phase;
using E.Loi.Edition.Generation.Helpers;
using E.Loi.Edition.Generation.Services;
using E.Loi.Edition.Generation.VMs;
using E.Loi.Edition.Generation.VMs.NodeType;
using E.Loi.Services.Repositories.Interfaces;
using mef.db.plf.edition.generation.Generation;
using mef.db.plf.edition.generation.License;
using System.Text.Json;
using System.Web;
namespace E.Loi.Edition.Generation.Generation.Plf;

public class GeneratePLF(IViewRenderService _viewRenderService, IUnitOfWork _unitOfWork)
{

    private void loadActiveNodesWithChildren(NodeLawVM node, List<Guid> activeNodes)
    {
        if (activeNodes.IndexOf(Guid.Parse(node.id)) < 0)
        {
            activeNodes.Add(Guid.Parse(node.id));
        }

        foreach (NodeLawVM child in node.children)
        {
            loadActiveNodesWithChildren(child, activeNodes);
        }
    }


    private void loadActiveNodes(NodeLawVM node, List<Guid> activeNodes)
    {
        if (activeNodes.IndexOf(Guid.Parse(node.id)) < 0)
        {
            activeNodes.Add(Guid.Parse(node.id));
        }
    }


    public async Task<FileStream> Print(SetDocumentData data)
    {
        LicenseAspose.setLicenseAsposeTotal("./License/Aspose.Total.NET.lic");
        Document document = new Document();
        CustomDocumentBuilder builder = new CustomDocumentBuilder(document);
        ManageEdition.ConfigBuilder(builder, data.language);


        var activeNodes = new List<Guid>();

        if (data.printings == true)
        {
            foreach (NodeLawVM node in data.nodeSelected)
            {
                loadActiveNodes(node, activeNodes);
            }
        }
        else
        {
            activeNodes = null;
        }

        List<string> annexes = new List<string>();
        for (var i = 0; i < data.nodes.Length; i++)
        {
            Guid nodeGuid = Guid.Parse(data.nodes[i].id);

            DataAccess.Models.Node gatherNode = await _unitOfWork.Nodes.GetByIdAsync(nodeGuid);

            Node_VM NodeVm = await _unitOfWork.Nodes.GetNode(gatherNode);

            await InsertNode(data.nodes[i], NodeVm, data.nodeTypes, builder, 0, i, data.language, data.law, data.phase, activeNodes!, (i == 0 ? false : true), false, annexes, data.authorised_nodes);
        }

        foreach (var annexe in annexes)
        {
            builder.InsertBreak(BreakType.SectionBreakNewPage);
            builder.InsertHtml(annexe);
        }
        AddHeaderAndFooterBrut(builder, document, data.law, data.phase, data.language, 0);

        document.UpdatePageLayout();
        string ext = "pdf";

        if (data.format != "pdf")
        {
            ext = "docx";
        }
        var filename = "files/" + String.Format("PLF_{0}_{1}." + ext, data.law.Number, data.language);
        document.Save(filename, data.format == "pdf" ? SaveFormat.Pdf : SaveFormat.Docx);

        FileStream stream = new FileStream(filename, FileMode.Open);
        return stream;
    }

    private async Task InsertNode(
    NodeLawVM node,
    Node_VM nodeVm,
    NodeTypeVM[] nodeTypes,
    CustomDocumentBuilder builder,
    int level,
    int iteration,
    string lang,
    LawVM law,
    PhaseVM phase,
    List<Guid> active = null!,
    bool addPageBreak = false,
    bool preview = false,
    List<string> annexes = null!,
    HashSet<Guid> autorisation = null!)
    {
        string header = string.Empty;
        string nodeType = ManageEdition.getNodeType(node.typeLabel!.Trim());


        NodeTypeVM nt = nodeTypes.FirstOrDefault(nt => nt.id == node.type)!;

        if (nt.isParentType)
        {
            var html = await _viewRenderService.RenderToStringAsync(lang + "/ProjetLoiTemplate", new ProjetLoiTemplate { Title = law.Number, Year = law.Year, Dir = (lang == "fr" ? "ltr" : "rtl") });
            builder.InsertHtml(html);
        }
        if (active == null || (active.IndexOf(Guid.Parse(node.id)) >= 0))
        {
            switch (nodeType)
            {
                case "projet":
                    {
                        break;
                    }
                case "partie":
                    {

                        switch (node.number)
                        {
                            case 1:
                                {
                                    header = "الجزء الأول";
                                    break;
                                }
                            case 2:
                                {
                                    header = "الجزء الثاني";
                                    break;
                                }
                            case 3:
                                {
                                    header = "الجزء الثالث";
                                    break;
                                }
                            case 4:
                                {
                                    header = "الجزء الرابع";
                                    break;
                                }
                            case 5:
                                {
                                    header = "الجزء الخامس";
                                    break;
                                }
                            case 6:
                                {
                                    header = "الجزء السادس";
                                    break;
                                }
                            case 7:
                                {
                                    header = "الجزء السابع";
                                    break;
                                }
                            case 8:
                                {
                                    header = "الجزء الثامن";
                                    break;
                                }
                            case 9:
                                {
                                    header = "الجزء التاسع";
                                    break;
                                }
                            case 10:
                                {
                                    header = "الجزء العاشر";
                                    break;
                                }
                            default:
                                {
                                    header = "الجزء " + (node.number);
                                    break;
                                }
                        }

                        var html = await _viewRenderService.RenderToStringAsync("PartieTemplate", new PartieTemplate { Header = header, Title = nodeVm.Label, Dir = (lang == "fr" ? "ltr" : "rtl") });
                        builder.InsertHtml(html);
                        break;
                    }
                case "chapitre":
                    {

                        switch (node.number)
                        {
                            case 1:
                                {
                                    header = "الباب الأول";
                                    break;
                                }
                            case 2:
                                {
                                    header = "الباب الثاني";
                                    break;
                                }
                            case 3:
                                {
                                    header = "الباب الثالث";
                                    break;
                                }
                            case 4:
                                {
                                    header = "";
                                    break;
                                }
                            case 5:
                                {
                                    header = "الباب الخامس";
                                    break;
                                }
                            case 6:
                                {
                                    header = "الباب السادس";
                                    break;
                                }
                            case 7:
                                {
                                    header = "الباب السابع";
                                    break;
                                }
                            case 8:
                                {
                                    header = "الباب الثامن";
                                    break;
                                }
                            case 9:
                                {
                                    header = "الباب التاسع";
                                    break;
                                }
                            case 10:
                                {
                                    header = "الباب العاشر";
                                    break;
                                }
                            default:
                                {
                                    header = "الباب " + (node.number);
                                    break;
                                }
                        }

                        var html = await _viewRenderService.RenderToStringAsync("ChapitreTemplate", new ChapitreTemplate { Header = header, Title = nodeVm.Label, Dir = (lang == "fr" ? "ltr" : "rtl") });
                        builder.InsertHtml(html);
                        break;
                    }
                case "article":
                    {
                        if (node.number == 1)
                        {
                            header = "المادة الأولى";
                        }
                        else
                        {

                            header = "المادة " + (node.number) + " " + ManageEdition.getBis(node.bis, lang);
                        }

                        var html = await _viewRenderService.RenderToStringAsync("ArticleTemplate", new ArticleTemplate { Header = header, Title = nodeVm.Label, Content = ManageEdition.UpdateStyle(nodeVm.Content) ?? string.Empty, Dir = (lang == "fr" ? "ltr" : "rtl") });
                        var decodedHtml = HttpUtility.HtmlDecode(html);
                        builder.InsertHtml(decodedHtml);
                        break;
                    }
                case "titre":
                    {
                        var html = await _viewRenderService.RenderToStringAsync("TitreTemplate", new TitreTemplate { Header = nodeVm.Label, Dir = (lang == "fr" ? "ltr" : "rtl") });
                        var decodedHtml = HttpUtility.HtmlDecode(html);
                        builder.InsertHtml(decodedHtml);
                        break;
                    }
                case "groupe":
                    {
                        var html = await _viewRenderService.RenderToStringAsync("GroupeTemplate", new GroupeTemplate { Header = nodeVm.Label, Dir = (lang == "fr" ? "ltr" : "rtl") });
                        var decodedHtml = HttpUtility.HtmlDecode(html);
                        builder.InsertHtml(decodedHtml);
                        break;
                    }
                case "annexe":
                    {
                        if (nodeVm.Content != String.Empty)
                        {
                            var html = await _viewRenderService.RenderToStringAsync("AnnexeTemplate", new GlobalTemplate { Title = string.Empty, Content = ManageEdition.UpdateStyleAnnexe(nodeVm.Content, lang) ?? string.Empty, Dir = (lang == "fr" ? "ltr" : "rtl") });
                            var decodedHtml = HttpUtility.HtmlDecode(html);
                            annexes.Add(decodedHtml);
                        }
                        break;
                    }
                case "tableau":
                    {
                        if (nodeVm.Content != "")
                        {
                            builder.InsertBreak(BreakType.SectionBreakNewPage);
                            var content = JsonSerializer.Deserialize<DouaneTarification>(nodeVm.Content);
                            var html = await _viewRenderService.RenderToStringAsync("TableTarificationTemplate", new DouaneTarification { rows = content!.rows });
                            var decodedHtml = HttpUtility.HtmlDecode(html);
                            builder.InsertHtml(decodedHtml);

                        }
                        break;
                    }

                default:
                    {
                        var html = await _viewRenderService.RenderToStringAsync("GlobalTemplate", new GlobalTemplate { Title = string.Empty, Content = ManageEdition.UpdateStyle(nodeVm.Content) ?? string.Empty, Dir = (lang == "fr" ? "ltr" : "rtl") });
                        var decodedHtml = HttpUtility.HtmlDecode(html);
                        builder.InsertHtml(decodedHtml);
                        break;
                    }
            }
        }

        if (!preview)
        {

            var i = 0;
            foreach (NodeLawVM child in node.children)
            {
                Guid Id = Guid.Parse(child.id);
                DataAccess.Models.Node gatherNode = await _unitOfWork.Nodes.GetByIdAsync(Id);

                Node_VM NodeVm = await _unitOfWork.Nodes.GetNode(gatherNode);

                await InsertNode(child, NodeVm, nodeTypes, builder, level + 1, i, lang, law, phase, active!, false, preview, annexes);
                i++;
            }
        }
    }



    private void AddHeaderAndFooterBrut(CustomDocumentBuilder builder, Document document, LawVM law, PhaseVM phase, string lang, int startingSection)
    {

        builder.MoveToSection(document.Sections.IndexOf(document.Sections[startingSection]));
        builder.MoveToHeaderFooter(HeaderFooterType.HeaderPrimary);
        if (lang == "fr")
        {
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            builder.InsertHtml("<span style='font-size:10px;'>" + "Projet de loi de finances " + " " + law.Number + " de l'année " + +law.Year + "</span>" + string.Join("", (new string[10]).Select(n => "&nbsp;")));
        }
        else
        {
            builder.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            if (phase.order == 3)
            {
                builder.InsertHtml("<span style='font-size:10px;'>" + " مشروع قانون المالية" + " " + law.Number + " لسنة " + law.Year + " كما وافق عليه مجلس النواب " + string.Join("", (new string[1]).Select(n => "&nbsp;")) + "</span>");
            }
            else if (phase.order == 5)
            {
                builder.InsertHtml("<span style='font-size:10px;'>" + " مشروع قانون المالية" + " " + law.Number + " لسنة " + law.Year + " كما وافق عليه مجلس المستشارين " + string.Join("", (new string[1]).Select(n => "&nbsp;")) + "</span>");
            }
            else
            {
                builder.InsertHtml("<span style='font-size:10px;'>" + " مشروع قانون المالية" + " " + law.Number + " لسنة " + law.Year + string.Join("", (new string[1]).Select(n => "&nbsp;")) + "</span>");

            }
        }


        builder.InsertHtml("<div style='width:150%;border-top:1px solid black;height:3px;'>&nbsp;</div>");

        builder.MoveToHeaderFooter(HeaderFooterType.FooterPrimary);

        builder.InsertHtml("<div style='width:150%;border-top:1px solid black;'>");

        if (lang == "fr")
        {
            builder.InsertHtml("<span style='display:inline:block;font-size:10px;margin-left:-150px;'>" + "Projet de loi de finances " + " " + law.Number + " de l'année " + law.Year + "</span>" + string.Join("", (new string[33]).Select(n => "&nbsp;")));
            var field = builder.InsertField("PAGE", string.Empty);
            field.Start.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            builder.InsertHtml("<span>" + string.Join("", (new string[68]).Select(n => "&nbsp;")) + "</span>");
            builder.InsertHtml("<span style='font-size:10px;'>Version de travail</span>" + string.Join("", (new string[1]).Select(n => "&nbsp;")));
        }
        else
        {
            builder.InsertHtml("<span style='font-size:10px;'>" + "صيغة العمل" + string.Join("", (new string[115]).Select(n => "&nbsp;")) + "</span>");

            var field = builder.InsertField("PAGE", string.Empty);
            field.Start.ParentParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            builder.InsertHtml("<span>" + string.Join("", (new string[50]).Select(n => "&nbsp;")) + "</span>");
            builder.InsertHtml("<span style='font-size:10px;'>" + " مشروع قانون المالية" + " " + law.Number + " لسنة " + law.Year + string.Join("", (new string[1]).Select(n => "&nbsp;")) + "</span>");
        }

        builder.InsertHtml("</div>");
    }
}

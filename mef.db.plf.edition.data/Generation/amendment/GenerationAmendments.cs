using Aspose.Words;
using Aspose.Words.Fonts;
using E.Loi.DataAccess.Vm.Amendment;
using E.Loi.Edition.Generation.Generation.amendment.ViewModels;
using E.Loi.Edition.Generation.Helpers;
using E.Loi.Edition.Generation.Services;
using E.Loi.Edition.Generation.VMs.amendment;
using E.Loi.Helpers.Enumarations;
using E.Loi.Helpers.Sheard;
using E.Loi.Services.Repositories.Interfaces;
using mef.db.plf.edition.generation.Generation;
using mef.db.plf.edition.generation.License;
using System.Web;
using Node = E.Loi.DataAccess.Models.Node;

namespace E.Loi.Edition.Generation.Generation.amendment;

public class GenerationAmendments(IViewRenderService _viewRenderService, IUnitOfWork _unitOfWork)
{


    Dictionary<int, string> mapArabicNumber = new Dictionary<int, string>()
        {
                { 0, ""},
                { 1,"الأول"},
                { 2,"الثاني"},
                { 3,"الثالث"},
                { 4,"الرابع"},
                { 5, "الخامس" },
                { 6, "السادس" },
                { 7, "السابع" },
                { 8, "الثامن" },
                { 9, "التاسع" },
                { 10, "العاشر" },
            };
    Dictionary<int, string> mapLatinNumber = new Dictionary<int, string> {
                { 0, ""},
                { 1,"I"},
                { 2,"II"},
                { 3,"III"},
                { 4,"IV"},
                { 5, "V" },
                { 6, "VI" },
                { 7, "VII" },
                { 8, "VIII" },
                { 9, "IX" },
                { 10, "X" },
            };
    Dictionary<int, string> mapAlphaArabic = new Dictionary<int, string> {
                { 0, ""},
                { 1,""},
                { 2,""},
                { 3,""},
                { 4,""},
                { 5,""},
                { 6,""},
                { 7, "" },
                { 8, "" },
                { 9, "" },
            };
    Dictionary<int, string> mapArabicBisF = new Dictionary<int, string> {
                { 0, ""},
                { 1,"المكررة"},
                { 2,"المكررة مرّتين"},
                { 3,"المكررة ثلاث مرات"},
                { 4,"المكررة أربع مرات"},
                { 5, "المكررة خمس مرات" },
                { 6, "المكررة ست مرات" },
                { 7, "المكررة سبع مرات" },
                { 8, "المكررة ثماني مرات" },
                { 9, "المكررة تسع مرات" },
                { 10, "المكررة عشر مرات" }

        };
    Dictionary<int, string> mapArabicBisM = new Dictionary<int, string> {
                { 0, ""},
                { 1,"المكرر"},
                { 2,"المكرر مرّتين"},
                { 3,"المكرر ثلاث مرات"},
                { 4,"المكرر أربع مرات"},
                { 5, "المكرر خمس مرات" },
                { 6, "المكرر ست مرات" },
                { 7, "المكرر سبع مرات" },
                { 8, "المكرر ثماني مرات" },
                { 9, "المكرر تسع مرات" },
                { 10, "المكرر عشر مرات" }

        };
    Dictionary<Guid, string> labelTypeNode;

    public async Task<FileStream> PrintTeamAmendments(SetAmendData data, ConfImpVoteComission config)
    {
        labelTypeNode = new Dictionary<Guid, string>
            {
               { config.IdParent,  "قانون المالية" },
               { config.IdPartie,  "الجزء"},
               { config.IdParagraphe,"" },
               {config.IdTitre,"الباب"},
               { config.IdArticle, "المادة" },
               { config.IdArticleMetier,"الفصل"},
               { config.IdAlinea,  "البند"},
               { config.IdTableau,  "جدول التعريفة الجمركية"},
               { config.IdAnnexe,  ""},
               { config.IdGroupe,  ""}
            };
        LicenseAspose.setLicenseAsposeTotal("./License/Aspose.Total.NET.lic");
        Aspose.Words.Document document = new();

        FontSettings.DefaultInstance.SubstitutionSettings.DefaultFontSubstitution.DefaultFontName = "Arial Unicode MS";
        CustomDocumentBuilder builder = new(document);
        ManageEdition.ConfigBuilderLandscape(builder, "ar");
        builder.PageSetup.RestartPageNumbering = true;
        string LawTitle = " مشروع قانون المالية رقم" + " " + data.lawNumber + " " + "للسنة المالية " + data.lawYear;

        var amendments = await _unitOfWork.Amendmnets.GetAmendmentsListAsync(data.amendmentsIds);

        List<AmendmentVM> amendmentVMs_ = new();
        foreach (var amendment in amendments)
        {
            var vote = await _unitOfWork.VoteAmendments.GetVoteAmendment(amendment.Id);
            amendmentVMs_.Add(new AmendmentVM
            {
                Id = amendment.Id.ToString(),
                NodeTitle = amendment.NodeTitle!,
                NodeId = amendment.NodeId,
                NodeTypeId = amendment.NodeTypeId,
                teamTitle = amendment.Team!,
                type = amendment.AmendmentIntent!,
                number = amendment.Number,
                amendmentIntent = amendment.AmendmentIntent!,
                title = amendment.Title!,
                content = amendment.Content!,
                originalContent = ManageEdition.UpdateStyle(amendment.OriginalContent!),
                justification = ManageEdition.UpdateStyle(amendment.Justification!),
                ordre = amendment.Order,
                numberSystem = amendment.NumberSystem,
                inFavor = vote?.InFavor,
                against = vote?.Against,
                neutral = vote?.Neutral,
                titleParagraphe=amendment.TitleParagraphe ??string.Empty,
                result = Constants.GetVoteResult(vote?.Result ?? "")
            });
        }
        AmendmentVM[] amendmentVMs = !data.WhithVote ? amendmentVMs_.OrderBy(am=>am.number).ToArray(): amendmentVMs_.ToArray();
        if (amendmentVMs.Length > 0 && amendmentVMs != null)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(data.TeamId);
            ManageEdition.AddAmendGrpGuardPage(builder, LawTitle, team?.Name!, data.WhithVote);
            builder.InsertBreak(BreakType.SectionBreakNewPage);
            foreach (var amendment in amendmentVMs)
            {
                await addAmendment(amendment, builder, config, data.WhithVote);
                builder.InsertBreak(BreakType.PageBreak);
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

        if (data.OutType == "pdf")
        {
            string ext = "pdf";
            var filename = "files/" + String.Format(data.WhithVote ? "VoteResult" : "AmendmentGroupe." + ext, "");
            document.Save(filename, SaveFormat.Pdf);
            FileStream stream = new FileStream(filename, FileMode.Open);
            return stream;
        }
        if (data.OutType == "docx")
        {
            string ext = "docx";
            var filename = "files/" + String.Format(data.WhithVote ? "VoteResult" : "AmendmentGroupe." + ext, "");
            document.Save(filename, SaveFormat.Docx);
            FileStream stream = new FileStream(filename, FileMode.Open);
            return stream;
        }
        throw new Exception("output type not yet supported");
    }

    private async Task addAmendment(AmendmentVM amendment, CustomDocumentBuilder builder, ConfImpVoteComission config, bool isVote)
    {
        {
            string label = amendment.NodeTitle;
            string parentLabel = "";
            var node = await _unitOfWork.Nodes.GetByIdAsync(amendment.NodeId);
            if (amendment.NodeTypeId == config.IdArticleMetier)
            {
                //todo
                var parentArticle = await getParentNodesArticle(node, config);
                if (parentArticle != null)
                {
                    if (parentArticle!.Nature != null)
                    {
                        if (parentArticle.Nature == NodeOrigin.Douane.ToString())
                        {
                            labelTypeNode[config.IdArticleMetier] = "الفصل";
                        }
                        else if (parentArticle.Nature == NodeOrigin.CGI.ToString())
                        {
                            labelTypeNode[config.IdArticleMetier] = "المادة الوظيفية";
                        }
                    }
                    parentLabel = parentArticle.Label;
                    parentLabel += "<br>" + printTitleType(parentArticle, config);
                }

            }
            string labelNode = string.Empty;
            if (label != null && label.Length > 0)
            {

                var str = printTitleType(node, config);
                labelNode = parentLabel + "<br>" + label + "<br>";//+str;
            }
            else
            {
                labelNode = parentLabel + "" + label + "<br>" + printTitleType(node, config);
            }
            if (isVote)
            {
                var html = await _viewRenderService.RenderToStringAsync("amendments/VoteAmendmentTemplate", amendment);
                var decodedHtml = HttpUtility.HtmlDecode(html);
                builder.InsertHtml(decodedHtml);
            }
            else
            {
                var html = await _viewRenderService.RenderToStringAsync("amendments/TeamAmendmentTemplate", new AmendmentsTemplate
                {
                    Number = amendment.number,
                    Intent = amendment.amendmentIntent,
                    OriginalContent = ManageEdition.UpdateStyle(amendment.originalContent),
                 
                    Content = ManageEdition.UpdateStyle(amendment.content.Replace("text-decoration-line", "text-decoration")),
                    Justification = ManageEdition.UpdateStyle(amendment.justification),
                    LabelNode = labelNode,
                    Team = amendment.teamTitle,
                    NumberSystem = amendment.numberSystem,
                    AmendmentTitle= $"{amendment.title} - {amendment.titleParagraphe} -"
                });
                var decodedHtml = HttpUtility.HtmlDecode(html);
                builder.InsertHtml(decodedHtml);
            }

        }
    }

    private async Task<Node> getParentNodesArticle(Node node, ConfImpVoteComission config)
    {
        if (node.TypeId == config.IdArticle) return node;
        if (node.ParentNode != null) return await getParentNodesArticle(node.ParentNode, config);
        return null!;
    }

    public string printTitleType(Node node, ConfImpVoteComission config)
    {

        if (config.IdParent == node.TypeId)
            return "";
        if (config.IdPartie == node.TypeId)
            return labelTypeNode[node.TypeId] + " " + mapArabicNumber[node.Number] + "<br>";
        if (config.IdParagraphe == node.TypeId)
            return "";
        if (config.IdTitre == node.TypeId)
            return labelTypeNode[node.TypeId] + " " + mapArabicNumber[node.Number] + "<br>";
        if (config.IdArticle == node.TypeId)
            return node.Number == 1 ? labelTypeNode[node.TypeId] + " الأولى " + " " + mapArabicBisF[node.Bis] : labelTypeNode[node.TypeId] + " " + node.Number + " " + mapArabicBisF[node.Bis];
        if (config.IdAlinea == node.TypeId)
            return labelTypeNode[node.TypeId] + " " + mapLatinNumber[node.Number];
        if (config.IdAnnexe == node.TypeId)
            return labelTypeNode[node.TypeId] + " " + mapAlphaArabic[node.Number];
        if (config.IdGroupe == node.TypeId)
            return "";
        if (config.IdTableau == node.TypeId)
            return labelTypeNode[node.TypeId];
        return string.Empty;
    }

    public string PrintTitleArticleMetier(Node node)
    {

        string labelBis = string.Empty;
        if (node.Nature == NodeOrigin.Douane.ToString())
        {
            labelBis = mapArabicBisM[node.Bis];
        }
        else if (node.Nature == NodeOrigin.CGI.ToString())
        {
            labelBis = mapArabicBisF[node.Bis];
        }
        else
        {
            labelBis = mapArabicBisM[node.Bis];
        }
        return labelTypeNode[node.TypeId] + " " + node.Number + " " + labelBis;

    }

}

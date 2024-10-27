using Aspose.Words;
using Aspose.Words.Fonts;
using E.Loi.DataAccess.Dtos;
using E.Loi.Edition.Generation.Helpers;
using E.Loi.Edition.Generation.Services;
using E.Loi.Services.Repositories.Interfaces;
using mef.db.plf.edition.generation.Generation;
using mef.db.plf.edition.generation.License;
using System.Web;

namespace E.Loi.Edition.Generation.Generation.VotingFile;

public class GenerateVoteResult(IUnitOfWork _uof, IViewRenderService _viewRenderService)
{
    public async Task<FileStream> PrintVoteAmendmentsResult(Guid lawId, Guid phaseId, string OutType)
    {
        LicenseAspose.setLicenseAsposeTotal("./License/Aspose.Total.NET.lic");
        Aspose.Words.Document document = new();
        FontSettings.DefaultInstance.SubstitutionSettings.DefaultFontSubstitution.DefaultFontName = "Arial Unicode MS";
        CustomDocumentBuilder builder = new(document);
        ManageEdition.ConfigBuilderLandscape(builder, "ar");
        builder.PageSetup.RestartPageNumbering = true;

        var law = await _uof.Laws.GetByIdAsync(lawId);
        string LawTitle = " مشروع قانون المالية رقم" + " " + law.Number + " " + "للسنة المالية " + law.Year;
        var amendments = await _uof.Amendmnets.GetVoteResult(lawId, phaseId);

        var teams = await _uof.Teams.GetTeams();
        List<VoteAmendmentsTeam> voteAmendments = new List<VoteAmendmentsTeam>();
        foreach (var team in teams)
        {

            VoteAmendmentsTeam voteAmendmentsTeam = new VoteAmendmentsTeam();
            var amendmentsTeam = amendments.Where(amd => amd.TeamId == team.Id).ToList();
            if (amendmentsTeam.Any())
            {
                voteAmendmentsTeam.TeamLaws.Add(
                    new TeamLaw
                    {
                        TeamName = team.Name,
                        LawName = LawTitle,
                    });
                voteAmendmentsTeam.VoteResult.AddRange(amendmentsTeam);
                voteAmendments.Add(voteAmendmentsTeam);
            }
        }
        var result = voteAmendments;

        if (amendments != null)
        {
            ManageEdition.AddVoteAmendmentsResultGuarddPage(builder, LawTitle);
            var html = await _viewRenderService.RenderToStringAsync("Voteresult/VoteResultAmendment", voteAmendments);
            var decodedHtml = HttpUtility.HtmlDecode(html);
            builder.InsertHtml(decodedHtml);
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
            string ext = "pdf";
            var filename = "files/" + System.String.Format("VotingFileSession." + ext, "");
            document.Save(filename, SaveFormat.Pdf);
            FileStream stream = new FileStream(filename, FileMode.Open);
            return stream;
        }
        if (OutType == "docx")
        {
            string ext = "docx";
            var filename = "files/" + System.String.Format("VotingFileSession." + ext, "");
            document.Save(filename, SaveFormat.Docx);
            FileStream stream = new FileStream(filename, FileMode.Open);
            return stream;
        }
        throw new Exception("output type not yet supported");
    }
}


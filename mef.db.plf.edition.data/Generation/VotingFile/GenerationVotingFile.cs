using Aspose.Words;
using Aspose.Words.Fonts;
using E.Loi.DataAccess.Dtos;
using E.Loi.Edition.Generation.Helpers;
using E.Loi.Edition.Generation.Services;
using E.Loi.Helpers.Enumarations;
using E.Loi.Services.Repositories.Interfaces;
using mef.db.plf.edition.generation.Generation;
using mef.db.plf.edition.generation.License;
using System.Web;

namespace E.Loi.Edition.Generation.VotingFile;

public class GenerationVotingFile(IUnitOfWork _uof, IViewRenderService _viewRenderService)
{
    public async Task<FileStream> PrintVotingFileSession(Guid SectionId, Guid LawId, string OutType)
    {
        LicenseAspose.setLicenseAsposeTotal("./License/Aspose.Total.NET.lic");
        Aspose.Words.Document document = new();
        FontSettings.DefaultInstance.SubstitutionSettings.DefaultFontSubstitution.DefaultFontName = "Arial Unicode MS";
        CustomDocumentBuilder builder = new(document);
        ManageEdition.ConfigBuilderLandscape(builder, "ar");
        builder.PageSetup.RestartPageNumbering = true;

        var law = await _uof.Laws.GetByIdAsync(LawId);
        string LawTitle = " مشروع قانون المالية رقم" + " " + law.Number + " " + "للسنة المالية " + law.Year;
        var data = await _uof.Nodes.GetSectionRubric(SectionId);
    
        if (data != null)
        {
            ManageEdition.AddVotingFileGuarddPage(builder, LawTitle, data.SectionNumber!);
            var html = await _viewRenderService.RenderToStringAsync("VotingFileSession/VotingFileTemplate", data);
            var decodedHtml = HttpUtility.HtmlDecode(html);
            builder.InsertHtml(decodedHtml);
            builder.InsertBreak(BreakType.SectionBreakNewPage);
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


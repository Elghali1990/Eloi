using Xceed.Words.NET;
namespace E.Loi.Components.Layout;

public partial class DownloadWord
{
    [Parameter]
    public List<LawVm> laws { get; set; } = new();

    private async Task DownloadeList()
    {
        var TemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Template/list-template.docx");
        var document = DocX.Load(TemplatePath);
        var ms = new MemoryStream();
        var table = document.Tables[0];
        int rowsCount = table.Rows.Count;
        foreach (var law in laws)
        {
            for (int i = 1; i < rowsCount; i++)
            {
                table.InsertRow(table.Rows[i], table.RowCount);
                var row = table.Rows[table.RowCount - 1];
                row.ReplaceText("٪النص٪", law.Label ?? "");
                row.ReplaceText("٪المرحلة٪", law.PhaseName ?? "");
                row.ReplaceText("٪الفريق٪", law.TeamName ?? "");
                row.ReplaceText("٪اللجنة٪", law.CommissionName ?? "");
            }
        }
        table.Rows[1].Remove();
        document.SaveAs(ms);
        byte[] byteArray = ms.ToArray();
        ms.Flush();
        ms.Close();
        ms.Dispose();
        var fileStream = new MemoryStream(byteArray);
        var fileName = "لائحة القوانين قيد الدرس.docx";
        using var streamRef = new DotNetStreamReference(stream: fileStream);
        using var rfStream = new DotNetStreamReference(stream: fileStream);
        await jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, rfStream);
    }
}

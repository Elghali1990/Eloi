using Xceed.Words.NET;

namespace E.Loi.Components.Pages.Statistics;


public partial class LawStatistic
{
    private List<StatisticAmendment> amendments = new();
    List<TeamVm> commissions = new();
    private List<LawVm> laws = new();
    List<Phase> phases = new();
    Guid LawId = Guid.Empty;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            commissions = await _teamRepository.GetCommissionsAsync();
        }
        catch (Exception ex)
        {
            Helpers.Trace.Trace.Logging(ex, nameof(OnInitializedAsync), nameof(LawStatistic));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    public async Task DownloadStatistic()
    {
        var TemplatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Template/statistic.docx");
        var document = DocX.Load(TemplatePath);
        var ms = new MemoryStream();
        var table = document.Tables[0];
        int rowsCount = table.Rows.Count;
        int counter = 0;
        amendments.Add(new StatisticAmendment
        {
            TeamName = "المجموع",
            Rejected = amendments.Sum(amd => amd.Rejected),
            Accepted = amendments.Sum(amd => amd.Accepted),
            Suppressed = amendments.Sum(amd => amd.Suppressed),
            Totale = amendments.Sum(amd => amd.Totale),
        });
        foreach (var amendment in amendments)
        {
            for (int i = 1; i < rowsCount; i++)
            {
                table.InsertRow(table.Rows[i], table.RowCount);
                var row = table.Rows[table.RowCount - 1];
                row.ReplaceText("٪الفريق٪", amendment.TeamName ?? "");
                row.ReplaceText("٪مرفوض٪", amendment.Rejected.ToString());
                row.ReplaceText("٪مقبول٪", amendment.Accepted.ToString());
                row.ReplaceText("٪مسحوب٪", amendment.Suppressed.ToString());
                row.ReplaceText("٪مجموع٪", amendment.Totale.ToString());
            }
        }
        table.Rows[1].Remove();

        document.SaveAs(ms);
        byte[] byteArray = ms.ToArray();
        ms.Flush();
        ms.Close();
        ms.Dispose();
        var fileStream = new MemoryStream(byteArray);
        var fileName = "إحصائيات.docx";
        using var streamRef = new DotNetStreamReference(stream: fileStream);
        using var rfStream = new DotNetStreamReference(stream: fileStream);
        await jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, rfStream);
    }

    private async Task getLaws(ChangeEventArgs e)
    {
        try
        {
            phases.Clear();
            if (!string.IsNullOrEmpty(e.Value!.ToString()))
            {
               Guid ComId = Guid.Parse(e.Value.ToString()!);
                laws = await _lawRepository.GetLawsToPrint(ComId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {getLaws}", nameof(LawStatistic));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    private async Task getLawPhases(ChangeEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Value!.ToString()))
        {
            phases.Clear();
            var selecteedLaw = await _lawRepository.GetByIdAsync(Guid.Parse(e.Value.ToString()!));
            LawId = selecteedLaw.Id;
            var phase = selecteedLaw.PhaseLawIds.Select(p => new Phase { Id = p.Phases.Id, Title = p.Phases.Title, Order = p.Phases.Order }).ToList();
            phases = phase.Where(p => p.Order > 1 && p.Order != 5).ToList();
        }
    }

    private async Task getStatistic(ChangeEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Value!.ToString()))
        {
            amendments = await _amendmentRepository.statistic(LawId, Guid.Parse(e.Value.ToString()!));
        }
    }

}



namespace E.Loi.Components.Pages.Laws;

public partial class FollowTextLaws
{
    List<LawVm> laws = new();
    List<LawVm> laws_ = new();
    List<int> pages = new();
    private int curentIndex = 0;
    List<TeamVm> teams = new();
    List<Phase> phases = new();
    List<Statut> status = new();
    List<TeamVm> commissions = new();
    private Guid BrowsLawId = Guid.Empty, LawDocumentId = Guid.Empty, BrowsLawAmendmentsId = Guid.Empty;
    private bool IsLoad = false;
    protected override async Task OnInitializedAsync()
    {
        IsLoad = true;
        laws = laws_ = await _lawRepository.getAllLawsForAll();
        teams = await _teamRepository.GetAllTeamsAsync();
        commissions = await _teamRepository.GetCommissionsAsync();
        phases = (await _phaseRepository.getAllAsync()).Where(s => s.Order > 0).ToList();
        status = (await _statuRepository.getAllStatus()).Where(s => s.Order > 0 && s.Order < 12).ToList();
        FillListOfPage(Constants.getPageSize(laws.Count));
        IsLoad = false;
    }

    /*
    * === Pagination ===
    */
    private void firstPage()
    {
        laws = laws_;
        laws = laws.Skip(0).Take(Constants.pageSize * 1).ToList();

    }
    private void lastPage()
    {
        laws = laws_.Skip((pages.Last() - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
    }
    private void paginate(int page)
    {

        if (page > curentIndex)
        {
            laws = laws_.Skip((page - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
        }
        else
        {
            laws = laws_;
            laws = laws.Skip(page - 1).Take(Constants.pageSize * (page - 1 == 0 ? 1 : page - 1)).ToList();
        }
        curentIndex = page;
    }
    void FillListOfPage(int count)
    {
        pages.Clear();
        for (int i = 1; i <= count; i++)
        {
            pages.Add(i);
        }
    }

    /*
     * === Filter law List ===
    */
    private void Filter(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            laws = laws_;
        else
        {
            if (filterBy == "Label")
                laws = laws_.Where(l => l.Label.Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Year")
                laws = laws.Where(l => l.Year.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Number")
                laws = laws_.Where(l => l.Number.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "StatuName")
                laws = laws_.Where(l => l.StatuName.Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "TeamName")
                laws = laws_.Where(l => l.TeamName.Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "CommissionName")
                laws = laws_.Where(l => l.CommissionName == e.Value!.ToString()).ToList();
        }
        FillListOfPage(Constants.getPageSize(laws.Count));
    }

    /*
    * === Browse Law ===
  */
    async Task BrowseLaw(Guid LawId_)
    {
        BrowsLawId = LawId_;
        await jsRuntime.InvokeVoidAsync(Constants.ShowCanvas, "canvasBrowsLaw");
    }

    /*
     * === Display Law Documents ===
    */
    private async Task ShowLawDocuments(Guid LawId_)
    {
        LawDocumentId = LawId_;
        await jsRuntime.InvokeVoidAsync(Constants.ShowCanvas, "canvasDocuments");
    }

    /*
     * === Brows Amendments ===
    */
    private async Task BrowsLawAmendments(Guid LawId_)
    {
        BrowsLawAmendmentsId = LawId_;
        await jsRuntime.InvokeVoidAsync(Constants.ShowCanvas, "canvasBrowsAmendment");
    }

}

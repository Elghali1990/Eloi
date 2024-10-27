namespace E.Loi.Components.Pages.Statistics;

public partial class Statistics
{

    #region === Global Variables ===
    int curentIndex = 0, pageSize = 10, selectedIndex = 0;
    List<Phase> phases = new();
    private bool isProcessing = false;
    private Guid LawId = Guid.Empty, LawDocumentId = Guid.Empty;
    List<StatisticsVM> statisticsCom = new();
    List<StatisticsVM> statisticsTeams = new();
    List<TeamVm> commissions = new();
    List<Legislative> legislatives = new();
    List<LegislativeYear> years = new();
    List<LegislativeSession> sessions = new();
    List<LawDetail> lawsDetail = new();
    List<LawDetail> lawsDetail_ = new();
    List<StatisticsDtos> statisticsReadOne = new();
    List<StatisticsDtos> statisticsReadTwo = new();
    List<int> pages = new();
    [SupplyParameterFromForm]
    public SearchDtos searchDtos { get; set; } = new();
    #endregion

    /*
     * === On Initialize Component ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            commissions = await _teamRepository.GetCommissionsAsync();
            phases = await _phaseRepository.getAllAsync();
            statisticsCom = await _statisticsRepository.StatisticsByCommittees();
            statisticsTeams = await _statisticsRepository.StatisticsByParliamentaryTeams();
            legislatives = await _legislativeRepository.getAll();
            statisticsReadOne = await _statisticsRepository.StatisticsReadOne();
            statisticsReadTwo = await _statisticsRepository.StatisticsReadTwo();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {OnInitializedAsync}", nameof(Statistics));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Get Commission Laws ===
    */
    async Task BrowseCommissionLaws(Guid commissionId)
    {
        try
        {
            isProcessing = true;
            lawsDetail = await _lawRepository.GetLawsByCommissionId(commissionId);
            setPageNumber();
            lawsDetail_ = lawsDetail;
            lawsDetail = lawsDetail.Skip(0).Take(pageSize).ToList();
            await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasBrowsLawsList");
            isProcessing = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {BrowseCommissionLaws}", nameof(Statistics));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    /*
     * === Handle Close Canvase ===
    */
    async Task CloseCanvase() => await jsRuntime.InvokeVoidAsync("HideCanvas", "canvasBrowsLawsList");


    /*
     * === Get Team Laws ===
    */
    async Task BrowseTeamLaws(Guid TeamId)
    {
        try
        {
            isProcessing = true;
            lawsDetail = await _lawRepository.GetLawsByTeamId(TeamId);
            setPageNumber();
            lawsDetail_ = lawsDetail;
            lawsDetail = lawsDetail.Skip(0).Take(pageSize).ToList();
            await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasBrowsLawsList");
            isProcessing = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {BrowseTeamLaws}", nameof(Statistics));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Show Law List ===
    */
    private async Task ShowList(List<Guid> Ids)
    {
        lawsDetail = await _lawRepository.GetLawByIds(Ids);
        setPageNumber();
        lawsDetail_ = lawsDetail;
        lawsDetail = lawsDetail.Skip(0).Take(pageSize).ToList();
        await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasBrowsLawsList");
    }

    /*
     * === filter ===
    */
    public async Task filterStatistics()
    {
        try
        {
            statisticsReadOne = await _statisticsRepository.StatisticsReadOne(searchDtos);
            statisticsReadTwo = await _statisticsRepository.StatisticsReadTwo(searchDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {filterStatistics}", nameof(Statistics));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Set Page Number ===
    */
    private void setPageNumber()
    {
        int count = Constants.getPageSize(lawsDetail.Count);
        pages.Clear();
        for (int i = 1; i <= count; i++)
            pages.Add(i);
    }

    /*
     * === Pagination ===
    */
    private void paginate(int page)
    {
        if (page > curentIndex)
        {
            lawsDetail = lawsDetail_.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        else
        {
            lawsDetail = lawsDetail_;
            lawsDetail = lawsDetail.Skip(page - 1).Take(pageSize * (page - 1 == 0 ? 1 : page - 1)).ToList();
        }
        curentIndex = page;
    }
    private void lastPage()
    {
        lawsDetail = lawsDetail_.Skip((pages.Last() - 1) * pageSize).Take(pageSize).ToList();
    }
    private void firstPage()
    {
        lawsDetail = lawsDetail_;
        lawsDetail = lawsDetail.Skip(0).Take(pageSize * 1).ToList();
    }
    /*
     * === Display Node Of Law ===
    */
    private async Task browsNodesOfLaw(Guid lawId_)
    {
        await jsRuntime.InvokeVoidAsync("HideCanvas", "canvasBrowsLawsList");
        LawId = lawId_;
        await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasBrowsLaw");
    }


    /*
     * === Refresh ===
    */
    private async Task Refresh()
    {
        try
        {
            statisticsReadOne = await _statisticsRepository.StatisticsReadOne();
            statisticsReadTwo = await _statisticsRepository.StatisticsReadTwo();
            searchDtos.StartDate = searchDtos.EndDate = null;
            searchDtos.IdCommission = Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {Refresh}", nameof(Statistics));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === display Law documens ===
    */
    private async Task ShowLawDocuments(Guid LawId_)
    {
        LawDocumentId = LawId_;
        await jsRuntime.InvokeVoidAsync("HideCanvas", "canvasBrowsLawsList");
        await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasDocuments");
    }
}

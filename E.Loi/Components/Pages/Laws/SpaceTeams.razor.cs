namespace E.Loi.Components.Pages.Laws;

public partial class SpaceTeams
{
    /*
     * === Global Variables ===
    */
    private List<LawVm> laws = new();
    private List<LawVm> _laws = new();
    private List<TeamVm> commissions = new();
    private List<TeamVm> teams = new();
    private List<Phase> _phases = new();
    bool IsLoad = false;
    List<int> pages = new();
    List<Statut> status = new();
    int curentIndex = 0, pageSize = 10;
    Guid LawId = Guid.Empty;


    /*
     * === On Component Load ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (!stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_GROUPE)))
            {
                _navigationManager.NavigateTo("/follow-laws");
            }
            var responseData = await _lawRepository.GetAllLawsTeamAsync("GetAllLawsTeamAsync");
            laws = responseData.ToList();
            _laws = laws.ToList();
            commissions = await _teamRepository.GetCommissionsAsync();
            teams = await _teamRepository.GetAllTeamsAsync();
            _phases = (await _phaseRepository.getAllAsync()).Where(p => p.Order > 0).ToList();
            status = (await _statuRepository.getAllStatus()).Where(s => s.Order > 0 && s.Order < 12).ToList();
            pages = Constants.FillListOfPage(Constants.getPageSize(laws.Count));
            IsLoad = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(OnInitializedAsync)}", nameof(SpaceTeams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    #region === Filter law list ===
    private void Filter(ChangeEventArgs e, string filterBy)

    {
        if (e.Value!.ToString() == string.Empty)
            laws = _laws;
        else
        {
            if (filterBy == "Label")
                laws = _laws.Where(l => l.Label.Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Year")
                laws = _laws.Where(l => l.Year.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Number")
                laws = _laws.Where(l => l.Number.Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Phase")
            {
                if (e.Value.ToString() != string.Empty)
                {
                    laws = _laws.Where(l => l.PhaseLawId == Guid.Parse(e.Value!.ToString()!)).ToList();
                }
            }

            if (filterBy == "Commission")
            {
                if (e.Value.ToString() != string.Empty)
                {
                    laws = _laws.Where(l => l.CommissionName == e.Value!.ToString()!).ToList();
                }
            }
            if (filterBy == "Statu")
            {
                if (e.Value.ToString() != string.Empty)
                {
                    laws = _laws.Where(l => l.StatuName == e.Value!.ToString()!).ToList();
                }
            }
            if (filterBy == "Team")
            {
                if (e.Value.ToString() != string.Empty)
                {
                    laws = _laws.Where(l => l.TeamName == e.Value!.ToString()!).ToList();
                }
            }
        }
    }
    #endregion


    #region === Pagination ===
    private void paginate(int page)
    {

        if (page > curentIndex)
        {
            laws = _laws.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        else
        {
            laws = _laws;
            int take = page - 1 == 0 ? 1 : page - 1;
            int skip = page - 1;
            laws = laws.Skip(skip).Take(pageSize * take).ToList();
        }
        curentIndex = page;
    }

    private void lastPage()
    {
        int lastPgae = pages.Last();
        laws = _laws.Skip((lastPgae - 1) * pageSize).Take(pageSize).ToList();
    }
    private void firstPage()
    {
        laws = _laws;
        laws = laws.Skip(0).Take(pageSize * 1).ToList();
    }
    #endregion

    private async Task ShowLawDocuments(Guid LawId_)
    {
        LawId = LawId_;
        await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasDocuments");
    }

}

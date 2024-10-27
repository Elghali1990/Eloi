namespace E.Loi.Components.Pages.Teams;

public partial class Teams
{

    /*
     * === Global Variables ===
    */
    private List<TeamVm> _teams = new();
    private List<TeamVm> teams = new();
    [SupplyParameterFromForm]
    private TeamVm _team { get; set; } = new();
    private bool IsLoad = false, IsMajority = false, IsMinority = false;
    List<int> pages = new();
    int curentIndex = 0;

    /*
     * === OnInitialaze Async ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadTeams();
            pages = Constants.FillListOfPage(Constants.getPageSize(teams.Count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${nameof(OnInitializedAsync)}", nameof(Teams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Bind List Of Teams ===
    */
    private async Task LoadTeams()
    {
        IsLoad = true;
        _teams.Clear();
        teams.Clear();
        _teams = teams = await _teamRepository.GetAllTeamsAsync();
        _teams = teams.Skip(0).Take(Constants.pageSize).ToList();
        IsLoad = false;
    }

    /*
     * === Show Modal ===
    */
    protected async Task ShowModal() => await jsRuntime.InvokeVoidAsync("ShowModal", "ModalTeams");

    /*
     * === Handle Add Or Update Team ===
    */
    private async Task EventAddOrEditTeam()
    {
        try
        {
            _team.IsMajority = IsMinority ? false : true;
            bool isSuccess = false;
            if (_team.Id == Guid.Empty)
                isSuccess = (await _teamRepository.CreateTeamAsync(_team, stateContainerService.user!.Id)).Flag;
            else
                isSuccess = (await _teamRepository.UpdateTeamAsync(_team, stateContainerService.user!.Id)).Flag;
            await RefreshData(isSuccess);
            await jsRuntime.InvokeVoidAsync("HideModal", "ModalTeams");
            await _traceService.insertTrace(new Trace { Operation = _team.Id == Guid.Empty ? "Add new Team" : "Update Team", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            _team = new();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(EventAddOrEditTeam)}", nameof(Teams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Handle Event Get Team Detail ===
    */
    private async Task HandleGetTeamById(Guid Id)
    {
        try
        {
            _team = _teams.FirstOrDefault(t => t.Id == Id)!;
            IsMinority = _team.IsMajority ? false : true;
            IsMajority = !IsMinority;
            await jsRuntime.InvokeVoidAsync("ShowModal", "ModalTeams");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(HandleGetTeamById)}", nameof(Teams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Set Majority ===
    */
    private void setMajority(ChangeEventArgs e)
    {
        IsMinority = false;
        IsMajority = !(bool)e.Value!;
    }
    private void setminority(ChangeEventArgs e)
    {
        IsMinority = !(bool)e.Value!;
        IsMajority = false;
    }

    /*
     * === Handle Event Delete Team ===
    */
    private async void HandleDeleteTeam(Guid teamId)
    {
        try
        {
            var response = await _teamRepository.DelteTeamAsync(teamId, stateContainerService.user!.Id);
            await RefreshData(response.Flag);
            await _traceService.insertTrace(new Trace { Operation = "Delete Team", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(HandleDeleteTeam)}", nameof(Teams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Filter List Teams ===
    */
    private void FilterTeams(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            _teams = teams;
        else if (filterBy == "Label")
            _teams = teams.Where(l => l.Label.Contains(e.Value!.ToString()!)).ToList();
        else
        {
            bool maj = e.Value!.ToString() == "الأغلبية" ? true : false;
            _teams = teams.Where(l => l.IsMajority == maj).ToList();
        }
        pages = Constants.FillListOfPage(Constants.getPageSize(_teams.Count));
    }
    /*
     * === Pagination ===
    */
    private void paginate(int page)
    {

        if (page > curentIndex)
            _teams = teams.Skip((page - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
        else
            _teams = teams.Skip(page - 1).Take(Constants.pageSize * (page - 1 == 0 ? 1 : page - 1)).ToList();
        curentIndex = page;
    }

    private void lastPage()
    {
        _teams = teams.Skip((pages.Last() - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
    }
    private void firstPage()
    {
        _teams = teams.Skip(0).Take(Constants.pageSize * 1).ToList();
    }

    public async Task RefreshData(bool Flag)
    {
        if (Flag)
        {
            await LoadTeams();
            toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = Blazored.Toast.Configuration.ToastPosition.BottomCenter; });
        }
        else
        {
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = Blazored.Toast.Configuration.ToastPosition.BottomCenter; });
        }
    }
}

namespace E.Loi.Components.Pages.Amendments;

public partial class ChangeAmendmentTeam
{
    [Parameter]
    public List<Guid> AmendmentIds
    {
        get;
        set;
    } = new();
    List<TeamVm> teams = new();

    private Guid TeamId = Guid.Empty;

    [Parameter]
    public EventCallback<bool> HandleEventChangeTeam
    {
        get;
        set;
    }

    public void SetTeamId(ChangeEventArgs e)
    {
        TeamId = Guid.Parse(e.Value!.ToString()!);
    }

    protected override async Task OnInitializedAsync()
    {
        teams = await _teamRepository.GetAllTeamsAsync();
    }

    private async Task HandleChangeTeam()
    {
        var response = await _amendmentRepository.ChangeAmendmentTeam(AmendmentIds, TeamId, stateContainerService.user.Id);
        await HandleEventChangeTeam.InvokeAsync(response.Flag);
    }
}

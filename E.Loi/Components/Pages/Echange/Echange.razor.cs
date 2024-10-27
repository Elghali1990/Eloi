
namespace E.Loi.Components.Pages.Echange;

public partial class Echange
{
    public List<TeamVm> allTeams = new List<TeamVm>();
    public List<TeamVm> teams = new List<TeamVm>();
    public List<Guid> selectedTeams = new List<Guid>();
    public Law law = new Law();
    public List<Phase> phases = new List<Phase>();
    public bool IsAmendmentsTeams = false, IsAmendmentsCommissions = false, IsAmendmentsSession = false, startSendTeams = false, startSendAmendments = false, voteAmendments = false, voteNodes = false, startSendVote = false;
    public FlatNode[] nodeSections = new FlatNode[0];
    public Guid PhaseId = Guid.Empty, sectionId = Guid.Empty;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            allTeams = await _teamRepository.GetAllTeamsForEchange();
            teams = allTeams.Where(t => (t.TeamType == TeamTypes.PARTIES.ToString() || t.TeamType == TeamTypes.PARTIES.ToString() || t.TeamType == TeamTypes.EXPLICIT_GROUP.ToString())).ToList();
            int year = DateTime.Now.Year + 1;
            law = await _lawRepository.getLawByYearAsync(year);
            phases = law.PhaseLawIds.Where(p => p.Statu == PhaseStatu.OPENED.ToString()).Select(x => x.Phases).ToList()!;
            nodeSections = await _nodeRepository.GetLawSections(law.Id, phases.First().Id, false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${nameof(OnInitializedAsync)}", nameof(Teams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    private void HandleSelectedTeams(ChangeEventArgs e, Guid teamId)
    {
        if ((bool)e.Value! == true)
            selectedTeams.Add(teamId);
        else
            selectedTeams.Remove(teamId);
    }
    private async Task SendTeams()
    {
        try
        {
            startSendTeams = true;
            var teamsDto = await _teamRepository.GetSelecteTeamsForEchange(selectedTeams, law.IdFinance ?? Guid.Empty);
            var response = await _echangeService.sendTeamsToMF(teamsDto);
            if (response.Flag)
            {
                toastService.ShowSuccess(Constants.SendTeamsMessage, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            selectedTeams.Clear();
            await jsRuntime.InvokeVoidAsync("uncheckAllCheckbox");
            startSendTeams = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${nameof(SendTeams)}", nameof(Teams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    private void setPhaseId(ChangeEventArgs e)
    {
        if (e.Value != string.Empty)
        {
            PhaseId = Guid.Parse(e.Value!.ToString()!);
        }
    }
    private void setSectionId(ChangeEventArgs e)
    {
        if (e.Value != string.Empty)
        {
            Console.WriteLine(e.Value);
            sectionId = Guid.Parse(e.Value!.ToString()!);
        }
    }
    private void setIsAmendmentsTeams(ChangeEventArgs e)
    {
        IsAmendmentsTeams = !(bool)e.Value!;
        IsAmendmentsCommissions = IsAmendmentsSession = false;

    }
    private void setIsAmendmentsCommissions(ChangeEventArgs e)
    {
        IsAmendmentsCommissions = !(bool)e.Value!;
        IsAmendmentsSession = IsAmendmentsTeams = false;
    }
    private void setIsAmendmentsSession(ChangeEventArgs e)
    {
        IsAmendmentsSession = !(bool)e.Value!;
        IsAmendmentsTeams = IsAmendmentsCommissions = false;
    }
    public async Task SendAmendments()
    {
        try
        {
            if (selectedTeams.Count > 0)
            {
                if (PhaseId != Guid.Empty)
                {
                    startSendAmendments = true;
                    var amendments = await _amendmentRepository.GetAmendmentsAsync(selectedTeams, law.Id, PhaseId);
                    if (amendments.Count > 0)
                    {
                        string phase = Constants.GetPhaseByOrder(phases.First(p => p.Id == PhaseId).Order);
                        var response = await _echangeService.getAmendmentsAsync(amendments, law.IdFinance ?? Guid.Empty, phase);
                        if ((response.Flag))
                        {
                            toastService.ShowSuccess(Constants.SendTeamsMessage, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                        }
                        else
                        {
                            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                        }
                    }
                    startSendAmendments = false;
                    selectedTeams.Clear();
                    PhaseId = Guid.Empty;
                    await jsRuntime.InvokeVoidAsync("uncheckAllCheckbox");
                }
                else
                {
                    toastService.ShowError("المرجو إختيار المرحلة .", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
            }
            else
            {
                toastService.ShowError("المرجو إختيار الفريق .", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${nameof(SendAmendments)}", nameof(Teams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    private void setIsVoteAmendments(ChangeEventArgs e)
    {
        voteAmendments = !(bool)e.Value!;
        voteNodes = false;
    }
    private void setIsVoteNodes(ChangeEventArgs e)
    {
        voteNodes = !(bool)e.Value!;
        voteAmendments = false;
    }

    private async Task sendVote()
    {
        try
        {
            bool isSuccess = false;
            startSendVote = true;
            if (sectionId == Guid.Empty)
            {
                toastService.ShowError("المرجو إختيار الجزء", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                if (voteAmendments)
                {
                    var voteAmendments = await _voteAmendmentRepository.GetVoteAmendmentsAsync(sectionId);
                    isSuccess = (await _echangeService.SendVoteAmendmentsAsync(voteAmendments.ToArray())).Flag;
                }
                else if (voteNodes)
                {
                    var voteNode = await _voteRepository.getVoteNodesAsync(sectionId);
                    isSuccess = (await _echangeService.SendVoteNodesAsync(voteNode.ToArray())).Flag;
                }
                else
                {
                    toastService.ShowError("المرجو إختيار نتائج التصويت.", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
            }
            if (isSuccess)
            {
                toastService.ShowSuccess(Constants.SendTeamsMessage, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            startSendVote = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${nameof(sendVote)}", nameof(Teams));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            startSendVote = false;
        }
    }
}

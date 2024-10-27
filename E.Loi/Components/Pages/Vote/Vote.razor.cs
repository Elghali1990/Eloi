namespace E.Loi.Components.Pages.Vote;
public partial class Vote
{

    /*
     * === Global Variables For Component ===
    */
    bool IsLoad = false, ShowVoteNodes = false, ShowVoteAmendments = false, isPrint = false, LoadNodes = false;
    string nodeContent = string.Empty, PhaseLawId = string.Empty;
    public List<string> PhaseLawIds = new();
    Guid IdNode = Guid.Empty;
    List<NodeVm> nodes = new();
    [Parameter] public string Id { get; set; } = string.Empty;
    NodeVoteVm[]? nodeVoteVms;
    IEnumerable<NodeVoteVm> selectedNode = new List<NodeVoteVm>();
    public Guid CurrentPhaseLawId = Guid.Empty;
    protected override void OnParametersSet() => setRoles();

    /*
     * === OnInitializedAsync Component ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(Id))
            {
                var law = await _lawRepository.GetByIdAsync(Guid.Parse(Id));
                //   PhaseLawIds = law.PhaseLawIds.Where(p=>p.Statu==PhaseStatu.OPENED.ToString()).Select(p => p.PhaseId.ToString()!.ToLower()).ToList()!;
                if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_SEANCE))
                {
                    CurrentPhaseLawId = law.PhaseLawIds.FirstOrDefault(p =>
                    (p.PhaseId.ToString()!.ToLower() == _phaseOptions.Value.PHASE_SEANCE_PLENIERE_1.ToLower()
                    || p.PhaseId.ToString()!.ToLower() == _phaseOptions.Value.PHASE_SEANCE_PLENIERE_2.ToLower())
                    && p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1);
                    // start TODO
                    if (law.PhaseLawIds.Count(p => p.Statu == PhaseStatu.OPENED.ToString()) == 0)
                        PhaseLawIds.Add(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1);
                    // end TODO
                }
                if (stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.MEMBER_COMMISSION))
                {
                    CurrentPhaseLawId = law.PhaseLawIds.FirstOrDefault(p =>
                    (p.PhaseId.ToString()!.ToLower() == _phaseOptions.Value.PHASE_COMMISSION_1.ToLower()
                    || p.PhaseId.ToString()!.ToLower() == _phaseOptions.Value.PHASE_COMMISSION_2.ToLower())
                    && p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1);
                    if (law.PhaseLawIds.Count(p => p.Statu == PhaseStatu.OPENED.ToString()) == 0)
                        PhaseLawIds.Add(_phaseOptions.Value.PHASE_COMMISSION_1);
                }
                if (CurrentPhaseLawId == Guid.Empty)
                {
                    CurrentPhaseLawId = law.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Empty;
                }
                PhaseLawIds.Add(CurrentPhaseLawId.ToString());
                PhaseLawId = CurrentPhaseLawId.ToString();
                nodes = await _nodeRepository.GetRecursiveChildren(law.Id, CurrentPhaseLawId, false);
                IsLoad = true;
            }
            setRoles();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on OnInitializedAsync", nameof(Vote));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private void setRoles()
    {
        stateContainerService.state!.ShowTeamAmendments = false;
        stateContainerService.state!.HasRightAddConsensusHarmonization = false;
        if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_SEANCE))
        {
            if (PhaseLawIds.Contains(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1.ToLower()) || PhaseLawIds.Contains(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_2.ToLower()))
            {
                if (CurrentPhaseLawId == Guid.Parse(PhaseLawId))
                {
                    stateContainerService.state!.HasRightvoteAmendment = true;
                    stateContainerService.state!.ShowTeamAmendments = false;
                }
                else
                {
                    stateContainerService.state!.ShowTeamAmendments = true;
                    stateContainerService.state!.HasRightvoteAmendment = false;
                }
            }
            else
            {
                stateContainerService.state!.HasRightvoteAmendment = false;
                stateContainerService.state!.ShowTeamAmendments = true;
            }
        }

        if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_COMMISSION))
        {
            if (PhaseLawIds.Contains(_phaseOptions.Value.PHASE_COMMISSION_1.ToLower()) || PhaseLawIds.Contains(_phaseOptions.Value.PHASE_COMMISSION_2.ToLower()))
            {
                if (CurrentPhaseLawId == Guid.Parse(PhaseLawId))
                {
                    stateContainerService.state!.HasRightvoteAmendment = true;
                    stateContainerService.state!.ShowTeamAmendments = false;
                }
                else
                {
                    stateContainerService.state!.ShowTeamAmendments = true;
                    stateContainerService.state!.HasRightvoteAmendment = false;
                }
            }
            else
            {
                stateContainerService.state!.HasRightvoteAmendment = false;
                stateContainerService.state!.ShowTeamAmendments = true;
            }
        }
        if (stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.DIRECTEUR_LEGISLATION))
        {

            if (CurrentPhaseLawId == Guid.Parse(PhaseLawId))
            {
                stateContainerService.state!.HasRightvoteAmendment = true;
                stateContainerService.state!.ShowTeamAmendments = false;
            }
            else
            {
                stateContainerService.state!.ShowTeamAmendments = true;
                stateContainerService.state!.HasRightvoteAmendment = false;
            }
        }
        StateHasChanged();
    }


    /*
     *=== Display Amendments List === 
    */
    public void DisplayAmendmentList(Guid nodeId)
    {
        ShowVoteAmendments = true;
        ShowVoteNodes = false;
        IdNode = nodeId;
        setRoles();
    }

    /*
     *=== Show Modal Vote == 
    */
    protected async Task ShowModelVote() => await jsRuntime.InvokeVoidAsync("ShowModal", "ModalVote");

    /*
     * === delete node vote ===
    */
    protected async Task DeleteVoteAsync()
    {
        try
        {
            if (selectedNode != null)
            {
                DeleteVoteVm vote = new();
                vote.Ids = selectedNode.Select(x => x.Id).ToList();
                var response = await _voteRepository.DeleteVoteAsync(vote);
                if (response.Flag)
                {
                    foreach (var node in nodeVoteVms!)
                    {
                        if (vote.Ids.Contains(node.Id))
                        {
                            node.InFavor = null;
                            node.Against = null;
                            node.Neutral = null;
                            node.Result = string.Empty;
                            node.VoteDate = null;
                        }
                    }
                    selectedNode = Enumerable.Empty<NodeVoteVm>();
                    toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    await _traceService.insertTrace(new Trace { Operation = "Delete Vote Nodes", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });

                }
                else
                {
                    toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on DeleteVoteAsync", nameof(Vote));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Reload Law By Phase === 
    */
    private async Task ChangePhaseLaw(List<string> phaseIds
        )
    {
        try
        {
            if (phaseIds.Count > 0)
            {
                IsLoad = false;
                var law = await _lawRepository.GetByIdAsync(Guid.Parse(Id));
                nodes = await _nodeRepository.GetRecursiveChildren(law.Id, Guid.Parse(phaseIds[0]), false);
                //  nodeVoteVms = await _nodeRepository.GetFlatNodes(law.Id, law.PhaseLawId);
                IsLoad = true;
                PhaseLawId = phaseIds[0];
                PhaseLawIds.Clear();
                PhaseLawIds = phaseIds;
                IdNode = Guid.Empty;
                setRoles();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "error on change phase law", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Get  Node Content === 
    */
    public async Task getNodeContent(Guid nodeID)
    {
        try
        {
            IdNode = nodeID;
            var nodeContentVm = await _nodeRepository.GetNodeContent(nodeID);
            if (nodeContentVm is not null) nodeContent = nodeContentVm.NodeContent;
            setRoles();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, "Error on handleLogin", nameof(Vote));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Toggle Boxe Vote Node === 
    */
    private async Task ToggleBoxVote(bool value)
    {
        ShowVoteAmendments = !value;
        ShowVoteNodes = value;
        if (ShowVoteNodes)
        {
            LoadNodes = ShowVoteNodes = true;
            ShowVoteAmendments = !ShowVoteNodes;
            nodeVoteVms = [];
            nodeVoteVms = await _nodeRepository.GetFlatNodes(Guid.Parse(Id), Guid.Parse(PhaseLawId));
            LoadNodes = false;
        }

    }

    /*
     * === Print Node Content === 
    */
    private async Task printNodeContent(Guid nodeId)
    {
        try
        {
            ShowVoteAmendments = ShowVoteNodes = false;
            setRoles();
            isPrint = true;
            await _printService.PrintNodeContent(nodeId);
            isPrint = false;
            await _traceService.insertTrace(new Trace { Operation = "Print Node Content", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(printNodeContent)}", nameof(Vote));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Updating Data After Vote ===
    */
    public async Task UpdatingDataAfterVote(bool isSuccess)
    {
        try
        {
            if (isSuccess)
            {
                nodeVoteVms = await _nodeRepository.GetFlatNodes(Guid.Parse(Id), Guid.Parse(PhaseLawId));
                await _traceService.insertTrace(new Trace { Operation = "Vote Nodes", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }

            selectedNode = Enumerable.Empty<NodeVoteVm>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(UpdatingDataAfterVote)}", nameof(Vote));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    public void UpdatedAmendmentsVoteResult(bool isSuccess) => refreshDataAfterVote(isSuccess);
    public void UpdatedAmendmentsAfterDeleteVote(bool isSuccess) => refreshDataAfterVote(isSuccess);


    private void refreshDataAfterVote(bool isSuccess)
    {
        if (isSuccess)
        {
            ShowVoteAmendments = true;
            ShowVoteNodes = false;
            setRoles();
        }
    }
}

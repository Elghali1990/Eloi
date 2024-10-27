namespace E.Loi.Components.Pages.Home;

public partial class Home
{

    #region === Variables ===
    bool IsLoad = false, IsShow = false, isPrintNodeContent = false;
    string AmendmentId = string.Empty, nodeContent = string.Empty, IdLaw = string.Empty, IdPhase = string.Empty/*, CurentPhaseLawId = string.Empty*/, SearchWords = string.Empty;
    DateTime? DateAmendment_1, DateAmendment_2;
    Guid IdNode = Guid.Empty;
    List<NodeVm> nodes = new();
    List<NodeVm> filteredNodes = new();
    public List<string> PhaseIds = new List<string>();
    [Parameter] public string Id { get; set; } = string.Empty;
    private string searchWords = string.Empty;
    #endregion

    /*
     * === OnParametersSetAsync ===
    */
    protected override async Task OnParametersSetAsync()
    {

        CheckHasRightAddAmendment(IdPhase.ToString()!, DateAmendment_1, DateAmendment_2);
        await Load();
    }

    /*
     * === OnInitializedAsync ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await Load();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"error on {nameof(OnInitializedAsync)}", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private async Task Load()
    {
        if (!string.IsNullOrEmpty(Id))
        {
            var law = await _lawRepository.GetByIdAsync(Guid.Parse(Id));
            DateAmendment_1 = law.DateFinAmendments1;
            DateAmendment_2 = law.DateFinAmendments2;
            IdPhase = law.PhaseLawIds.LastOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId.ToString() ?? Guid.Empty.ToString();
            var phase = law.PhaseLawIds.Where(p => p.Statu == PhaseStatu.OPENED.ToString()).OrderByDescending(p => p.Phases?.Order).Select(p => p.PhaseId.ToString()).First();
            nodes = await _nodeRepository.GetRecursiveChildren(law.Id, Guid.Parse(IdPhase), false);
            IdPhase = phase!;
            PhaseIds.Add(phase!);
            IsLoad = true;
            IdLaw = law.Id.ToString();
            //Todo
            CheckHasRightAddAmendment(IdPhase ?? string.Empty, DateAmendment_1, DateAmendment_2);
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
            IsShow = false;
            CheckHasRightAddAmendment(IdPhase, DateAmendment_1, DateAmendment_2);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, $"Error on {nameof(getNodeContent)}", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Display Amendments List ===
    */
    public void handle_DisplayAmendmentList(Guid nodeId)
    {
        IdNode = nodeId;
        IsShow = false;
        CheckHasRightAddAmendment(IdPhase, DateAmendment_1, DateAmendment_2);
    }

    /*
     * === Handel Add Amendment ===
    */
    public void handle_AddAmendment(Guid nodeID)
    {
        IsShow = false;
        IdNode = nodeID;
        AmendmentId = string.Empty;
        CheckHasRightAddAmendment(IdPhase, DateAmendment_1, DateAmendment_2);
    }

    public void HandleEventAddAmendmentSupplementary(Guid nodeID)
    {
        IsShow = false;
        IdNode = nodeID;
        AmendmentId = string.Empty;
        CheckHasRightAddAmendment(IdPhase, DateAmendment_1, DateAmendment_2);
    }
    /*
     * === Edit Amendment ===
    */
    public void EditAmendment(string Id) => AmendmentId = Id;


    /*
     * === show end hide box upload amendments ===
    */
    private void handleUploadAmendments(bool value) => IsShow = value;


    /*
     * === Reload Law By Phase ===
    */
    private async Task ChangePhaseLaw(List<string> phaseIds)
    {
        try
        {
            if (phaseIds.Count > 0)
            {
                IdPhase = phaseIds[0];
                PhaseIds.Clear();
                PhaseIds.Add(IdPhase);
                IsLoad = false;
                var law = await _lawRepository.GetByIdAsync(Guid.Parse(Id));
                nodes = await _nodeRepository.GetRecursiveChildren(law.Id, Guid.Parse(phaseIds[0]), false);
                IsLoad = true;
                IdLaw = law.Id.ToString();
                IsShow = false;
                stateContainerService.state = new() { NodeVm = null!, ShowViewListAmendments = false, ShowAddAmendment = false, ShowViewNodeContent = false, ShowViewAllAmendments = false };
                CheckHasRightAddAmendment(phaseIds[0], law.DateFinAmendments1, law.DateFinAmendments2);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"error on {nameof(ChangePhaseLaw)}", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Check Has Right Add Amendment ===
    */
    private void CheckHasRightAddAmendment(string phase, DateTime? Date_Amendment_1, DateTime? Date_Amendment_2)
    {
        stateContainerService.state!.ShowTeamAmendments = true;
        if (_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1 == phase.ToUpper() || _phaseOptions.Value.PHASE_SEANCE_PLENIERE_2 == phase.ToUpper())
        {
            stateContainerService.state!.HasRightAddAmendment = true;
            stateContainerService.state.HasRightShowViewListAmendments = true;
            stateContainerService.state!.HasRightUploadAmendments = true;
            stateContainerService.state!.HasRightAddAmendmentSupplementary = false;
        }
        else
        {
            stateContainerService.state!.HasRightUploadAmendments = false;
            if (phase.ToUpper() == _phaseOptions.Value.PHASE_COMMISSION_1)
            {
                if (DateAmendment_1 == null)
                {
                    stateContainerService.state!.HasRightAddAmendment = true;
                    stateContainerService.state.HasRightShowViewListAmendments = false;
                }
                else
                {
                    if (DateTime.UtcNow > Date_Amendment_1 || DateAmendment_1 ==null)
                    {
                        stateContainerService.state!.HasRightAddAmendment = false;
                        stateContainerService.state.HasRightShowViewListAmendments = true;
                    }
                    else
                    {
                        stateContainerService.state!.HasRightAddAmendment = true;
                        stateContainerService.state.HasRightShowViewListAmendments = false;
                    }
                }
            }
            if (phase.ToUpper() == _phaseOptions.Value.PHASE_COMMISSION_2)
            {
                if (DateTime.UtcNow > Date_Amendment_2 || DateAmendment_2 == null)
                {
                    stateContainerService.state!.HasRightAddAmendment = true;
                    stateContainerService.state.HasRightShowViewListAmendments = false;
                }
                else
                {

                    if (DateTime.UtcNow > Date_Amendment_2)
                    {
                        stateContainerService.state!.HasRightAddAmendment = false;
                        stateContainerService.state.HasRightShowViewListAmendments = true;
                    }
                    else
                    {
                        stateContainerService.state!.HasRightAddAmendment = true;
                        stateContainerService.state.HasRightShowViewListAmendments = false;
                    }
                }
            }

            stateContainerService.state.HasRightAddAmendmentSupplementary = !stateContainerService.state.HasRightAddAmendment;
        }

    }

    /*
     * === Handel Show Display All Amendments ===
    */
    private void DisplayAllAmendments(Guid nodeId)
    {
        IdNode = nodeId;
        IsShow = false;
        CheckHasRightAddAmendment(IdPhase, DateAmendment_1, DateAmendment_2);
    }

    /*
     * === Handle Print Node Content ===
    */
    private async Task handlePrintNodeContent(Guid NodeId)
    {
        try
        {
            IsShow = false;
            isPrintNodeContent = !isPrintNodeContent;
            await _traceService.insertTrace(new Trace { Operation = "Print Node Content", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            await _printService.PrintNodeContent(NodeId);
            isPrintNodeContent = !isPrintNodeContent;
            CheckHasRightAddAmendment(IdPhase, DateAmendment_1, DateAmendment_2);
        }
        catch (Exception ex)
        {
            isPrintNodeContent = false;
            _logger.LogError(ex.Message, "error on printNodeContent", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private List<NodeVm> ApplySearch(NodeVm node, string query)
    {
        var node_ = node.Label!.Contains(query);


        if (node.childrens != null)
        {
            foreach (var child in node.childrens)
            {
                ApplySearch(child, query);
                filteredNodes.Add(child);
            }
        }
        return filteredNodes;
    }


    private void PerformSearch(ChangeEventArgs e)
    {
        List<NodeVm> nodes_ = new();
        if (e.Value!.ToString() != string.Empty)
        {
            foreach (var node in nodes)
            {
                nodes_.AddRange(ApplySearch(node, e.Value.ToString() ?? ""));

            }
        }
        var r = nodes_;
    }

}

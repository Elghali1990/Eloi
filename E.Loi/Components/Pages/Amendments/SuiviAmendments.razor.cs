namespace E.Loi.Components.Pages.Amendments;
public partial class SuiviAmendments
{
    /*
     * === Component Globals Variabls ===
    */
    [Parameter] public string Id { get; set; } = string.Empty;
    bool isDownload = false, IsLoad = false, isPrint = false;
    List<NodeVm> nodes { get; set; } = new();
    List<AmendmentsListVm> amendmentsListVms = new();
    List<AmendmentsListVm> amendmentsListVms_ = new();
    string NodeContent = string.Empty, AmendmentId = string.Empty;
    public List<string> PhaseIds = new();
    List<FlatNode> parentsNode = new();
    private Guid AmendId = Guid.Empty, PhaseId = Guid.Empty, CurrentPhaseId = Guid.Empty, IdNode = Guid.Empty;
    private bool checkAll = false, checkboxCheckAll = false;
    private List<Guid> AmdIds = new();
    List<TeamVm> teams = new();
    protected override void OnParametersSet() => OnSelectNode();

    /*
     * === On Componente Load ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(Id))
            {
                await LoadNodesOfLaw(Guid.Parse(Id));
                OnSelectNode();
            }
            teams = await _teamRepository.GetAllTeamsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "error OnInitializedAsync", nameof(SuiviAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private async Task LoadNodesOfLaw(Guid lawId)
    {
        IsLoad = false;
        var law = await _lawRepository.GetByIdAsync(lawId);
        //   PhaseIds = law.PhaseLawIds.Where(i => i.Statu!.Equals(PhaseStatu.OPENED.ToString())).Select(i => i.PhaseId.ToString()).ToList()!;
        if (CurrentPhaseId == Guid.Empty)
        {
            if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_SEANCE))
            {
                CurrentPhaseId = law.PhaseLawIds.FirstOrDefault(p =>
                (p.PhaseId.ToString()!.ToLower() == _phaseOptions.Value.PHASE_SEANCE_PLENIERE_1.ToLower()
                || p.PhaseId.ToString()!.ToLower() == _phaseOptions.Value.PHASE_SEANCE_PLENIERE_2.ToLower())
                && p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1);
                if (law.PhaseLawIds.Count(i => i.Statu!.Equals(PhaseStatu.OPENED.ToString())) == 0)
                {
                    //  PhaseIds.Add(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1);
                }
            }
            if (stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.MEMBER_COMMISSION) || stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.DIRECTEUR_LEGISLATION))
            {
                CurrentPhaseId = law.PhaseLawIds.FirstOrDefault(p =>
                (p.PhaseId.ToString()!.ToLower() == _phaseOptions.Value.PHASE_COMMISSION_1.ToLower()
                || p.PhaseId.ToString()!.ToLower() == _phaseOptions.Value.PHASE_COMMISSION_2.ToLower())
                && p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1);
                if (law.PhaseLawIds.Count(i => i.Statu!.Equals(PhaseStatu.OPENED.ToString())) == 0)
                {
                    // PhaseIds.Add(_phaseOptions.Value.PHASE_COMMISSION_1);
                }
            }
        }
        PhaseId = CurrentPhaseId;
        PhaseIds.Add(CurrentPhaseId.ToString());
        nodes = await _nodeRepository.GetRecursiveChildren(law.Id, PhaseId, false);
        IsLoad = true;
    }



    private void OnSelectNode()
    {
        stateContainerService.state!.ShowTeamAmendments = true;
        stateContainerService.state!.HasRightvoteAmendment = false;
        if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_SEANCE))
        {

            if (CurrentPhaseId == Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1) || CurrentPhaseId == Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_2))
                stateContainerService.state.HasRightAddConsensusHarmonization = true;
        }
        if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_COMMISSION))
        {

            if (CurrentPhaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1) || CurrentPhaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_2))
                stateContainerService.state.HasRightAddConsensusHarmonization = true;
        }

        if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.DIRECTEUR_LEGISLATION))
        {
            stateContainerService.state.HasRightAddConsensusHarmonization = true;
        }
    }

    /*
     *  === Get Node Content ===
    */
    public async Task getNodeContent(Guid nodeID)
    {
        try
        {
            IdNode = nodeID;
            var nodeContentVm = await _nodeRepository.GetNodeContent(nodeID);
            stateContainerService.state!.ShowTeamAmendments = true;
            if (nodeContentVm is not null) NodeContent = nodeContentVm.NodeContent;
            OnSelectNode();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, "Error on handleLogin", nameof(SuiviAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Handel Event Add and Edit Amendment ===
    */
    public void handle_AddAmendment(Guid nodeID)
    {
        OnSelectNode();
        stateContainerService.state!.ShowTeamAmendments = true;
        IdNode = nodeID;
    }
    public void EditAmendment(string Id) => AmendmentId = Id;


    /*
     * === Print Amendment Pdf ===
    */
    private async Task Print_PDF() => await PrintAmendments("pdf");

    /*
     * === Print Amendments Word ===
    */
    private async Task Print_WORD() => await PrintAmendments("docx");


    /*
     * === Method get amendments doc ===
    */
    private async Task PrintAmendments(string OutType)
    {
        try
        {
            isPrint = true;
            var url = _navigationManager.Uri.Split('/');
            var law = await _lawRepository.GetByIdAsync(Guid.Parse(url[url.Length - 1]));
            SetAmendData amendData = new();
            amendData.lawNumber = law.Number;
            amendData.lawYear = law.Year;
            amendData.OutType = OutType;
            amendData.TeamId = stateContainerService.user!.TeamId;
            amendData.amendmentsIds = AmdIds;
            var bytes = await _editionRepository.PrintTeamAmendments(amendData);
            if (bytes is not null)
            {
                await _printService.DownloadAmendments(bytes, "تعديلات - " + law.Label, OutType);
            }
            isPrint = false;
            await _traceService.insertTrace(new Trace { Operation = OutType == "docx" ? "Print Amendments Word" : "Print Amendments Pdf", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });

        }
        catch (Exception ex)
        {
            isPrint = false;
            _logger.LogError(ex.Message, "Error On Get amendment file stream", nameof(SuiviAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Publish Amendments ===
    */
    private async Task PublishAmendments()
    {
        try
        {
            SetAmendmentStatuVm vm = new()
            {
                Ids = AmdIds,
                AmendmentStatu = AmendmentsStatus.PUBLIC.ToString(),
                UserId = stateContainerService.user!.Id
            };
            var response = await _amendmentRepository.SetAmendmentsAsync(vm);
            if (response.Flag)
            {
                foreach (var Id in vm.Ids)
                {
                    var amendment = amendmentsListVms!.FirstOrDefault(a => a.Id == Id);
                    if (amendment is not null) amendment.AmendmentsStatus = "عام";
                    StateHasChanged();
                }
                toastService.ShowSuccess(Constants.MessageSuccessPublishAmendment, settings => { settings.ShowCloseButton = false; settings.Position = Blazored.Toast.Configuration.ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on publish amendments", nameof(SuiviAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Reloading Law By Phase ===
    */
    private async Task ChangePhaseLaw(List<string> phaseIds)
    {
        try
        {
            if (PhaseIds.Count > 0)
            {
                CurrentPhaseId = Guid.Parse(phaseIds[0]);
                await LoadNodesOfLaw(Guid.Parse(Id));
                PhaseId = Guid.Parse(phaseIds[0]);
                PhaseIds.Clear();
                PhaseIds = phaseIds;
                OnSelectNode();
                stateContainerService.state = new() { NodeVm = null!, ShowViewListAmendments = false, ShowAddAmendment = false, ShowViewNodeContent = false };
            }
            OnSelectNode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "error on change phase law", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    /*
     * === Handle Event Get Amendment Detail ===
    */
    public async Task GetAmendmentDetails(Guid Id)
    {
        AmendId = Id;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModalDetail");
    }

    /*
     * === Show Submited Amendments ===
    */

    private async Task handleShowSubmitedAmendments(Guid nodeId)
    {
        try
        {
            isDownload = true;
            parentsNode = (await _nodeRepository.GetFlatParents(nodeId)).ToList();
            amendmentsListVms = amendmentsListVms_ = await _amendmentRepository.GetSubmitedAmendmentsListAsync(nodeId);
            isDownload = false;
            stateContainerService.state!.ShowTeamAmendments = true;
            stateContainerService.state.ShowAmendmentsSupplementary = false;
            OnSelectNode();
        }
        catch (Exception ex)
        {
            isDownload = false;
            _logger.LogError(ex.Message, "Error on get Submited Amendments detail", nameof(SuiviAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    private async Task ShowAmendmentsSupplementary(Guid nodeId)
    {
        try
        {
            isDownload = true;
            parentsNode = (await _nodeRepository.GetFlatParents(nodeId)).ToList();
            amendmentsListVms = amendmentsListVms_ = await _amendmentRepository.GetSubmitedAmendmentsListAsync(nodeId);
            isDownload = false;
            stateContainerService.state!.ShowTeamAmendments = true;
            stateContainerService.state.ShowAmendmentsSupplementary = true;
            OnSelectNode();
        }
        catch (Exception ex)
        {
            isDownload = false;
            _logger.LogError(ex.Message, "Error on get Submited Amendments detail", nameof(SuiviAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    /*
     * === Print Node Content ===
    */
    private async Task handlePrintNodeContent(Guid NodeId)
    {
        try
        {
            isPrint = !isPrint;
            await _printService.PrintNodeContent(NodeId);
            stateContainerService.state!.ShowTeamAmendments = true;
            isPrint = !isPrint;
            OnSelectNode();
        }
        catch (Exception ex)
        {
            isPrint = false;
            _logger.LogError(ex.Message, $"Error on {handlePrintNodeContent}", nameof(SuiviAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private void Filter(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            amendmentsListVms = amendmentsListVms_;
        else
        {
            if (filterBy == "NumberSystem")
                amendmentsListVms = amendmentsListVms_.Where(l => l.NumberSystem.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Number")
                amendmentsListVms = amendmentsListVms_.Where(l => l.Number.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Order")
                amendmentsListVms = amendmentsListVms_.Where(l => l.Order.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "NodeTitle")
                amendmentsListVms = amendmentsListVms_.Where(l => l.NodeTitle!.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Title")
                amendmentsListVms = amendmentsListVms_.Where(l => l.Title!.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "AmendmentType")
                amendmentsListVms = amendmentsListVms_.Where(l => l.AmendmentIntent!.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "AmendmentsStatus")
                amendmentsListVms = amendmentsListVms_.Where(l => l.AmendmentsStatus!.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Team")
                amendmentsListVms = amendmentsListVms_.Where(l => l.Team! == e.Value!.ToString()!).ToList();
            if (filterBy == "VoteResult")
                amendmentsListVms = amendmentsListVms_.Where(l => l.VoteResult == e.Value!.ToString()!).ToList();
        }
        //FillListOfPage(Constants.getPageSize(amendmentsListVms.Count));
    }

    private void CheckAllAmendments()
    {
        AmdIds.Clear();
        if (checkAll)
        {
            checkAll = false;
            checkboxCheckAll = false;
        }
        else
        {
            AmdIds = amendmentsListVms.Select(amd => amd.Id).ToList();
            checkAll = true;
            checkboxCheckAll = true;
        }
    }

    private void CheckAmendment(ChangeEventArgs e, Guid Id)
    {
        if ((bool)e.Value! == true)
        {
            checkAll = false;
            AmdIds.Add(Id);
            if (AmdIds.Count == amendmentsListVms.Count)
            {
                checkboxCheckAll = true;
            }
        }
        else
        {
            AmdIds.Remove(Id);
            checkboxCheckAll = false;
        }
    }

    public async Task handleReassignmentAmendment(Guid IdAmendment, Guid NodeId)
    {
        AmendId = IdAmendment;
        IdNode = NodeId;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModelReassignment");
    }

    public async Task EventChangeAmendmentNodeId(bool isSuccess)
    {
        await jsRuntime.InvokeVoidAsync(Constants.HideModal, "ModelReassignment");
        if (isSuccess)
        {
            await handleShowSubmitedAmendments(IdNode);
            toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
        else
        {
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


}

using DocumentFormat.OpenXml.Drawing.Charts;
using E.Loi.DataAccess.Vm.Node;

namespace E.Loi.Components.Pages.Amendments;

public partial class Amendments
{
    /*
     * === Component Variables ===
    */
    string? nodeContent, Title, NodeRefContent, AmendmentIntent, displayNone, _nodeRefContent;
    private string Justification { get; set; } = string.Empty;
    private string AmendmentText { get; set; } = string.Empty;
    [Parameter] public Guid NodeId { get; set; } = Guid.Empty;
    [Parameter] public string AmendmentId { get; set; } = string.Empty;
    [Parameter] public EventCallback<bool> handleCancelAddAmendment { get; set; }
    [Parameter] public EventCallback<bool> handleEventAddAmendment { get; set; }
    int Number, NodeNumber = 0, Bis = 0, orderParagraphe = 0;
    Guid TeamId = Guid.Empty, RefLawId = Guid.Empty, nodeTypeId = Guid.Empty;
    List<LawVm> LawsVm = new();
    List<NodeVm> nodes = new();
    AmendmentsListVm[]? amendmentsListVms;
    private bool IsLoad = false, IsLoadAmendment = false, SelectedIntent = false, IsPrintNodeContent = false, IsPrintAmendmentContent = false, IsPrintJustification = false, hideBis = false;
    IEnumerable<AmendmentsListVm>? selectedAmendments;
    private bool AmendmentIntentAddition = false, isAmendableAdd = false, isAmendableDelete = false, isAmendableEdit = false;
    List<FlatNode> parentsNode = new();
    private Dictionary<string, string> nodeTypes = new();


    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            StateHasChanged();
        }
    }
    /*
     * === Component OnParametersSetAsync(Life cycle) === 
    */


    private async Task getAmendmentIntentsNodeType(Guid NodeTypeId)
    {
        var nodeType = await _nodeTypeRepository.GetNodeTypeByIdAsync(NodeTypeId);
        if (nodeType is not null)
        {
            isAmendableAdd = nodeType.IsAmendableAdd;
            isAmendableDelete = nodeType.IsAmendableDelete;
            isAmendableEdit = nodeType.IsAmendableEdit;
            if (!isAmendableAdd && !isAmendableDelete && !isAmendableEdit) stateContainerService.state!.HasRightAddAmendment = true;
            nodeTypes.Clear();
            var nodeTypeVm = await _nodeTypeRepository.GetNodeTypesWithNodeHierarchies(nodeType.Id);
            if (nodeTypeVm is not null)
            {
                foreach (var child in nodeTypeVm.Childrens)
                {
                    nodeTypes.Add(child.Label, child.Id.ToString());
                }
            }
        }
    }
    protected override async Task OnParametersSetAsync()
    {
        try
        {

            IsLoad = IsLoadAmendment = true;
            AmendmentIntentAddition = false;
            if (NodeId != Guid.Empty && string.IsNullOrEmpty(AmendmentId))
            {
                var parents = await _nodeRepository.GetFlatParents(NodeId);
                parentsNode = parents.ToList();
                var nodeContentVm = await _nodeRepository.GetNodeContent(NodeId);
                Title = nodeContentVm.Label;
                if (nodeContentVm is not null)
                {
                    await getAmendmentIntentsNodeType(nodeContentVm.NodeTypeId);
                    nodeContent = AmendmentText = nodeContentVm.NodeContent;
                    if (stateContainerService.amendmentType.ToString() == AmendmentTypes.CONSENSUS.ToString())
                    {
                        amendmentsListVms = (await _amendmentRepository.GetAmendmentsForCluster(NodeId))?.ToArray();
                    }
                    if (stateContainerService.amendmentType.ToString() == AmendmentTypes.HARMONIZATION.ToString())
                    {
                        var node = await _nodeRepository.GetNodeByID(NodeId);
                        amendmentsListVms = (await _amendmentRepository.GetAmendmentsForCluster(node.LawId, node.PhaseLawId))?.ToArray();
                    }

                    Number = 0;
                    Justification = string.Empty;
                }
            }
            TeamId = stateContainerService.user.TeamsDtos!.First().Id;
            IsLoad = IsLoadAmendment = false;
        }
        catch (Exception e)
        {
            IsLoad = IsLoadAmendment = false;
            _logger.LogError(e.Message, $"Error on {nameof(OnParametersSetAsync)}", nameof(Amendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    /*
     * === Component OnInitializedAsync ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {

            IsLoad = IsLoadAmendment = true;
            if (!string.IsNullOrEmpty(AmendmentId))
            {
                var amendment = await _amendmentRepository.GetAmendmentByIdAsync(Guid.Parse(AmendmentId));
                if (amendment != null)
                {
                    Title = amendment.Title;
                    Number = amendment.Number;
                    Justification = amendment.Justification;
                    nodeContent = amendment.OriginalContent;
                    AmendmentText = amendment.Content;
                    NodeId = amendment.NodeId;
                    orderParagraphe = amendment.ParagrapheOrder;
                    var node = await _nodeRepository.GetNodeByID(amendment.NodeId);
                    Bis = node.Bis;
                    NodeNumber = node.Number;
                    nodeTypeId = node.TypeId;
                    setAmendmentType(amendment.AmendmentIntent);
                    if (amendment.AmendmentType == AmendmentTypes.CONSENSUS.ToString() || amendment.AmendmentType == AmendmentTypes.HARMONIZATION.ToString())
                    {
                        var data = await _amendmentRepository.GetClusterAmendments(Guid.Parse(AmendmentId));
                        if (data != null) amendmentsListVms = data.ToArray();
                    }

                    orderParagraphe = amendment.ParagrapheOrder;
                }
            }
            IsLoad = IsLoadAmendment = false;
        }
        catch (Exception e)
        {
            IsLoad = IsLoadAmendment = false;
            _logger.LogError(e.Message, $"Error on {nameof(OnInitializedAsync)}", nameof(Amendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Dispaly and hide box chose law ref ===
    */
    private void ToggleBoxSelectRefLaw(ChangeEventArgs e)
    {
        AmendmentIntentAddition = e.Value!.Equals(Constants.ADDITION) ? true : false;
        AmendmentIntent = e.Value!.ToString();
        displayNone = "d-none";
    }

    /*
     * === Get Reference from SGI Law ===
    */
    protected async Task getRefLawNodes()
    {
        if (RefLawId != Guid.Empty)
        {
            await jsRuntime.InvokeVoidAsync(Constants.ShowModal, "ModalRefLaw");
        }
    }

    /*
     * === Get Node Content from sgi law ===
    */
    protected async Task getNodeContent(Guid NodeId)
    {
        try
        {
            var nodeContentVm = await _nodeRepository.GetNodeContent(NodeId);
            if (nodeContentVm is not null)
            {
                NodeRefContent = nodeContentVm.NodeContent;
                _nodeRefContent = nodeContentVm.NodeContent;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, $"Error on {nameof(getNodeContent)}", nameof(Amendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Set Original Text ===
    */
    protected void SetTextOriginal(string content) => nodeContent = AmendmentText = content;

    private void setTeamId(ChangeEventArgs e) => TeamId = Guid.Parse(e.Value?.ToString()!);


    /*
     * === Add Or Update Amendment ===
    */

    private bool ValidateForm()
    {
        RequiredFieldsErrors.Clear();
        bool validateForm = true;
        RequiredFieldsErrors.Clear();
        if (string.IsNullOrEmpty(AmendmentIntent))
        {
            RequiredFieldsErrors.Add(Constants.MessageIntentRequired);
            validateForm = false;
        }
        if (string.IsNullOrEmpty(orderParagraphe.ToString()) || orderParagraphe == 0)
        {
            validateForm = false;
            RequiredFieldsErrors.Add("المرجو إختيار الفقرة");
        }
        if (string.IsNullOrEmpty(nodeContent))
        {
            validateForm = false;
            RequiredFieldsErrors.Add(Constants.MessageOriginalTextRequired);
        }
        if (string.IsNullOrEmpty(AmendmentText))
        {
            validateForm = false;
            RequiredFieldsErrors.Add(Constants.MessageAmendmentContentRequired);
        }
        if (TeamId == Guid.Empty)
        {
            validateForm = false;
            RequiredFieldsErrors.Add(Constants.MessageTeamRequired);
        }
        if (stateContainerService.amendmentType == AmendmentTypes.CONSENSUS || stateContainerService.amendmentType == AmendmentTypes.HARMONIZATION)
        {
            if (selectedAmendments is null)
            {
                validateForm = false;
                RequiredFieldsErrors.Add(Constants.MessageSelectAmendments);
            }
        }

        if (AmendmentIntent == Constants.ADDITION && nodeTypeId == Guid.Empty)
        {
            validateForm = false;
            RequiredFieldsErrors.Add(Constants.MessageSelectNodeType);
        }
        if (AmendmentIntent == Constants.ADDITION && NodeNumber <= 0)
        {
            validateForm = false;
            RequiredFieldsErrors.Add(Constants.MessageNumberNode);
        }
        return validateForm;
    }
    protected async Task SaveAmendment()
    {
        try
        {
            bool isSuccess = false, existAmendmentByNumber = false;
            if (ValidateForm())
            {
                var nodeExist = await _nodeRepository.GetNodeByTypeByNumberByBis(NodeId,nodeTypeId, NodeNumber, Bis);
                if (AmendmentIntent == Constants.ADDITION  && nodeExist.Flag) {
                    toastService.ShowError("Node Exist", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    return;
                }
                var _parentNode = await _nodeRepository.GetNodeByIdAsync(NodeId);
                if (string.IsNullOrEmpty(AmendmentId))
                {

                    isSuccess = await CreateAmendment(_parentNode.PhaseLawId, _parentNode.LawId);
                    await _traceService.insertTrace(new Trace { Operation = "Add Amendment", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                }
                else
                {
                    isSuccess = await UpdateAmendment();
                    await _traceService.insertTrace(new Trace { Operation = "Update Amendments", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });

                }
                if (isSuccess && !existAmendmentByNumber)
                {
                    toastService.ShowSuccess(Constants.MessageSuccessAddAmendment, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_GROUPE))
                    {
                        stateContainerService.state!.HasRightAddAmendment = !stateContainerService.state.HasRightAddAmendmentSupplementary;
                        stateContainerService.state.ShowAddAmendment = false;
                        StateHasChanged();
                    }
                    else
                    {
                        stateContainerService.state!.ShowAddAmendment = false;
                    }
                    stateContainerService.state!.ShowViewListAmendments = true;
                    ResetForm();
                    await handleEventAddAmendment.InvokeAsync(true);
                }
                if (!isSuccess && !existAmendmentByNumber)
                {
                    toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(SaveAmendment)} ", nameof(Amendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private void ResetForm()
    {
        Number = NodeNumber = Bis = 0;
        Title = Justification = nodeContent = AmendmentText = string.Empty;
    }

    /*
     * === Create Amendment ===
    */
    private async Task<bool> CreateAmendment(Guid PhaseLawId, Guid LawId)
    {
        bool IsAmendmentSession = false;
        AmendmentVm amendment = new();
        if (AmendmentIntent == Constants.ADDITION)
        {
            var createdNode = await CreateVirtualNode(PhaseLawId, LawId);
            var lawPLF = await _lawRepository.GetByIdAsync(createdNode.LawId);
            if (lawPLF.Category == "PLF")
            {
                await UpdateNodeContent(createdNode);
            }
            amendment.NodeId = createdNode.Id;
        }
        else
        {
            amendment.NodeId = NodeId;
        }
        if (_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1.Trim() == PhaseLawId.ToString().ToUpper() || _phaseOptions.Value.PHASE_SEANCE_PLENIERE_2.Trim() == PhaseLawId.ToString().ToUpper())
        {
            IsAmendmentSession = true;
        }
        amendment.TeamId = (TeamId != Guid.Empty ? TeamId : stateContainerService.user!.TeamsDtos!.FirstOrDefault()!.Id);
        amendment.AmendmentIntent = AmendmentIntent!;
        amendment.AmendmentType = stateContainerService.amendmentType.ToString();
        amendment.Title = Title!;
        amendment.ParagrapheOrder = orderParagraphe;
        amendment.ParagreapheTitle = $"الفقرة {orderParagraphe}";
        amendment.Content = AmendmentText!;
        amendment.Justification = Justification!;
        amendment.IsAmendementRattrape = stateContainerService.state!.HasRightAddAmendmentSupplementary;
        amendment.IsAmendementSeance = IsAmendmentSession;
        amendment.OriginalContent = nodeContent!;
        amendment.CreatedBy = stateContainerService.user!.Id;
        amendment.AmendmentIds = selectedAmendments != null ? selectedAmendments.Select(a => a.Id).ToList() : null;
        return (await _amendmentRepository.CreateAmendmantAsync(amendment)).Flag;
    }

    /*
     * === Create Virtuel Node ===
    */
    private async Task<Node> CreateVirtualNode(Guid PhaseLawId, Guid LawId)
    {
        var virtuelNode = new CreateNodeVm();
        virtuelNode.Label = string.Empty;
        virtuelNode.TypeId = nodeTypeId;
        virtuelNode.Content = nodeContent!;
        virtuelNode.OriginalContent = nodeContent!;
        virtuelNode.PhaseLawId = PhaseLawId;
        virtuelNode.LawId = LawId;
        virtuelNode.Bis = Bis;
        virtuelNode.Number = NodeNumber;
        virtuelNode.ParentNodeId = NodeId;
        virtuelNode.CreatedBy = stateContainerService.user!.Id;
        var createdNode = await _nodeRepository.CreateVirtuelNode(virtuelNode);
        return createdNode;
    }

    /*
     * === Update Virtual Node Content ===
    */
    private async Task UpdateNodeContent(Node node)
    {
        await _nodeRepository.UpdateNodeContent(new NodeContentVm()
        {
            Id = node.Id,
            NodeTypeId = nodeTypeId,
            Label = node.Label,
            NodeContent = node.Content,
            IdFinace = node.Id,
            Number = NodeNumber,
            Bis = Bis,
        },
        stateContainerService.user.Id
        );
    }

    /*
     * === Update Amendment ===
    */
    private async Task<bool> UpdateAmendment()
    {
        var amendment_ = await _amendmentRepository.GetAmendmentByIdAsync(Guid.Parse(AmendmentId));
        AmendmentVm amendment = new();
        //amendment.Number = Number;
        amendment.AmendmentIntent = AmendmentIntent!;
        amendment.Title = Title!;
        amendment.Content = AmendmentText!;
        amendment.Justification = Justification!;
        amendment.OriginalContent = nodeContent!;
        amendment.UpdatedBy = stateContainerService.user!.Id;
        amendment.ParagrapheOrder = orderParagraphe;
        amendment.ParagreapheTitle = $"الفقرة {orderParagraphe}";
        if (selectedAmendments != null) amendment.AmendmentIds = selectedAmendments.Select(a => a.Id).ToList();
        return (await _amendmentRepository.UpdateAmendmantAsync(Guid.Parse(AmendmentId), amendment)).Flag;
    }
    /*
     * === Handle Select Referances Law ===
    */
    protected async Task handleSelectLawsRef(ChangeEventArgs e)
    {
        try
        {
            LawsVm.Clear();
            displayNone = string.Empty;
            LawsVm = await _lawRepository.GetLawsByCategoryAsync(e.Value?.ToString()!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(handleSelectLawsRef)}", nameof(Amendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Print Text Editor Content ===
    */
    private async Task printNodeContentEditor()
    {
        IsPrintNodeContent = true;
        await PrintEditorContent(new EditorContent() { Content = nodeContent! }, nameof(printNodeContentEditor));
        IsPrintNodeContent = false;
    }
    private async Task printAmendmentContentEditor()
    {
        IsPrintAmendmentContent = true;
        await PrintEditorContent(new EditorContent() { Content = AmendmentText! }, nameof(printAmendmentContentEditor));
        IsPrintAmendmentContent = false;
    }
    private async Task printJustificationContentEditor()
    {
        IsPrintJustification = true;
        await PrintEditorContent(new EditorContent() { Content = Justification! }, nameof(printJustificationContentEditor));
        IsPrintJustification = false;
    }
    private async Task PrintEditorContent(EditorContent editorContent, string methodeName)
    {
        try
        {
            var bytes = await _editionRepository!.PrintEditorContent(editorContent);
            if (bytes is not null)
            {
                var fileStream = new MemoryStream(bytes);
                var fileName = "editor-content.pdf";
                using var streamRef = new DotNetStreamReference(stream: fileStream);
                using var rfStream = new DotNetStreamReference(stream: fileStream);
                await jsRuntime!.InvokeVoidAsync("downloadFileFromStream", fileName, rfStream);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {methodeName}", nameof(Amendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Cancel Add Amendment ===
    */
    protected async void CancleAddAmendment()
    {
        ResetForm();
        AmendmentId = string.Empty;
        stateContainerService.state!.HasRightAddAmendment = true;
        stateContainerService.state.HasRightAddAmendment = true;
        stateContainerService.state.ShowViewListAmendments = true;
        stateContainerService.state.ShowAddAmendment = true;
        if (!stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_GROUPE))
        {
            stateContainerService.state.HasRightAddConsensusHarmonization = true;
        }
        await handleCancelAddAmendment.InvokeAsync(false);
    }

    /*
     * === select Node Type ===
    */
    protected void GetSelectedNodeType(ChangeEventArgs e)
    {
        nodeTypeId = Guid.Parse(e.Value?.ToString()!);
        hideBis = e.Value?.ToString()?.Trim() == Constants.Clause.Trim() ? true : false;
    }

    void setAmendmentType(string amendmentIntent)
    {
        switch (amendmentIntent)
        {
            case "ADDITION":
                isAmendableAdd = SelectedIntent = true;
                AmendmentIntent = Constants.ADDITION;
                break;
            case "DELETION":
                isAmendableDelete = SelectedIntent = true;
                AmendmentIntent = Constants.DELETION;
                break;
            case "MODIFICATION":
                isAmendableEdit = SelectedIntent = true;
                AmendmentIntent = Constants.MODIFICATION;
                break;
            default:
                isAmendableEdit = isAmendableAdd = isAmendableDelete = SelectedIntent = false;
                break;
        }
    }
}

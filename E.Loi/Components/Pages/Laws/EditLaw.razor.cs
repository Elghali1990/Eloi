namespace E.Loi.Components.Pages.Laws;

public partial class EditLaw
{
    /*
     * === Component Variables ===
    */
    [Parameter] public string? Id { get; set; }
    string? nodeContent, nodeTitle, nodeTypelabel;
    List<NodeVm> nodes = new();
    List<LawVm> laws = new();
    public Law law { get; set; }
    Guid TypeId, PhaseLawId, lawId, ParentNodeId, nodeId;
    bool hasTitle = false, hasContent = false, showBoxContent = false, IsUpdate = false, haspresentation = false, IsLoad = false, startProcessing = false, isDisabel = false, showLawAction = true;
    List<FlatNode> parentsNode = new();
    List<NodeTypeVm> nodeTypes = new();

    /*
     * === On Initialze Component ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (!string.IsNullOrEmpty(Id))
            {
                IsLoad = true;
                law = await _lawRepository.GetByIdAsync(Guid.Parse(Id));
                if (law is null)
                {

                    nodes = null;
                    IsLoad = false;
                }
                else
                {
                    if (law.Category == "PLF")
                    {
                        PhaseLawId = law.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString() && (p.Phases.Order == 0 || p.Phases.Order == 1))?.PhaseId ?? Guid.Empty;
                        if (PhaseLawId != Guid.Empty)
                        {
                            nodes = await _nodeRepository.GetRecursiveChildren(law.Id, PhaseLawId, false);
                        }
                        else
                        {
                            nodes = await _nodeRepository.GetRecursiveChildren(law.Id, Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1), false);
                        }

                    }
                    else
                    {
                        PhaseLawId = law.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Empty;
                        lawId = law.Id;
                        nodes = await _nodeRepository.GetRecursiveChildren(law.Id, PhaseLawId, false);
                    }
                }
            }
            IsLoad = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(OnInitializedAsync)}", nameof(EditLaw));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Set Value Of NodeId ===
    */
    private async Task SelectNode(Guid nodeId)
    {
        ParentNodeId = nodeId;
        if (law.Category == "PLF")
        {
            showBoxContent = true;
            var parents = await _nodeRepository.GetFlatParents(nodeId);
            parentsNode = parents.ToList();
            var nodeContentVm = await _nodeRepository.GetNodeContent(nodeId);
            if (nodeContentVm != null)
            {
                nodeContent = nodeContentVm.NodeContent;
            }
        }
    }


    /*
     * === Get Node Type ===
    */
    private async Task AddNewNode(Guid Id)
    {
        showBoxContent = true;
        isDisabel = IsUpdate = false;
        nodeTitle = nodeContent = string.Empty;
        TypeId = Guid.Empty;
        nodeTypes = await _nodeTypeRepository.GetNodeTypesAsync();
        parentsNode = (await _nodeRepository.GetFlatParents(ParentNodeId)).ToList();
        showLawAction = false;
    }

    /*
     * === Insert new Node ===
    */
    private async Task HandleAddNewNode()
    {
        try
        {
            hasTitle = string.IsNullOrEmpty(nodeTitle);
            hasContent = string.IsNullOrEmpty(nodeContent);
            bool IsSuccess = false;
            startProcessing = true;
            if (!hasTitle && !hasContent)
            {
                if (!IsUpdate)
                {
                    CreateNodeVm nodeVm = new();
                    nodeVm.Label = nodeTitle!;
                    nodeVm.TypeId = TypeId;
                    nodeVm.Content = nodeContent!;
                    nodeVm.OriginalContent = nodeContent!;
                    nodeVm.PhaseLawId = PhaseLawId;
                    nodeVm.LawId = lawId;
                    nodeVm.ParentNodeId = ParentNodeId;
                    nodeVm.CreatedBy = stateContainerService.user!.Id;
                    var node = await _nodeRepository.CreateNode(nodeVm);
                    IsSuccess = node?.Id != null ? true : false;
                }
                else
                {
                    NodeContentVm nodeContentVm = new NodeContentVm();
                    nodeContentVm.Id = nodeId;
                    nodeContentVm.Label = nodeTitle!;
                    nodeContentVm.NodeContent = nodeContent!;
                    nodeContentVm.NodeTypeId = TypeId;
                    var response = await _nodeRepository.UpdateNodeContent(nodeContentVm, stateContainerService.user!.Id);
                    IsSuccess = response.Flag;
                }
            }
            if (IsSuccess)
            {
                nodes = await _nodeRepository.GetRecursiveChildren(lawId, PhaseLawId, false);
                toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                await _traceService.insertTrace(new Trace { Operation = IsUpdate ? "Update Node Content" : "Add New Node", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            IsUpdate = startProcessing = false;
            nodeTitle = nodeContent = nodeTypelabel = string.Empty;
            showBoxContent = !showBoxContent;
            TypeId = Guid.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(HandleAddNewNode)}", nameof(EditLaw));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    /*
     * === Delete Node ===
    */
    private async Task setIdNodeToDelete(Guid Id, bool hasChildrens)
    {
        if (hasChildrens)
        {
            await jsRuntime.InvokeVoidAsync(Constants.ShowModal, "ModalNodeHasChildrens");
        }
        else
        {
            nodeId = Id;
            await jsRuntime.InvokeVoidAsync(Constants.ShowModal, "ModalConfirmDeleteNode");
        }
    }
    private async Task DeleteNode()
    {
        try
        {
            startProcessing = true;
            var response = await _nodeRepository.DeleteNode(nodeId, stateContainerService.user!.Id);
            if (response.Flag)
            {
                await _traceService.insertTrace(new Trace { Operation = "Delete  Node", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                nodes = await _nodeRepository.GetRecursiveChildren(lawId, PhaseLawId, false);
                toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            startProcessing = false;
            await jsRuntime.InvokeVoidAsync(Constants.HideModal, "ModalConfirmDeleteNode");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(DeleteNode)}", nameof(EditLaw));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    // text-align: right !important;

    /*
     * === Edit Node Content ===
    */
    private async Task EditNodeContent(NodeVm node)
    {
        try
        {
            showLawAction = false;
            isDisabel = node.ParentId != null ? false : true;
            showBoxContent = true;
            nodeTypes = await _nodeTypeRepository.GetNodeTypesAsync();
            var parents = await _nodeRepository.GetFlatParents(node.Id);
            parentsNode = parents.ToList();
            var nodeContentVm = await _nodeRepository.GetNodeContent(node.Id);
            IsUpdate = true;
            nodeId = node.Id;
            nodeTypelabel = "تحديث العقدة";
            if (nodeContentVm != null)
            {
                TypeId = nodeContentVm.NodeTypeId;
                nodeTitle = nodeContentVm.Label;
                nodeContent = $"<div style='direction: ltr  !important;'>{nodeContentVm.NodeContent}</div>";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {EditNodeContent}", nameof(EditLaw));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    /*
     * === Select Node Type
    */
    private void SelectNodeType(ChangeEventArgs e)
    {
        nodeTypelabel = "إضافة :" + nodeTypes.FirstOrDefault(t => t.Id.ToString().ToLower() == e.Value!.ToString()?.ToLower())?.Label;
        TypeId = Guid.Parse(e.Value?.ToString()!);
    }

    /*
     * === Handle Cancel ===
    */
    private void HandleCancel()
    {
        showBoxContent = false;
        ParentNodeId = TypeId = Guid.Empty;
    }
}
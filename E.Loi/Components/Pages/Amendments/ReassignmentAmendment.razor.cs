namespace E.Loi.Components.Pages.Amendments;

public partial class ReassignmentAmendment
{
    [Parameter]
    public Guid NodeId
    {
        get;
        set;
    }

    [Parameter]
    public Guid PhaseId
    {
        get;
        set;
    }

    [Parameter]
    public Guid AmendmentId
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<bool> HandleEventChangeAmendmentNodeId
    {
        get;
        set;
    }



    public bool loadNodes
    {
        get;
        set;
    } = false;

    protected bool StartProcessing = false;
    public List<NodeVm>? nodes { get; set; }
    public Node? node;
    protected override async Task OnParametersSetAsync()
    {
        if (AmendmentId != Guid.Empty && PhaseId != Guid.Empty && NodeId != Guid.Empty)
        {
            loadNodes = true;
            node = await _nodeRepository.GetNodeByID(NodeId);
            nodes = await _nodeRepository.GetRecursiveChildren(node.LawId, PhaseId, false);
            loadNodes = false;

        }
    }

    public async Task SelectedNode(Guid nodeId)
    {
        node = await _nodeRepository.GetNodeByID(nodeId);
    }

    private async Task ChangeAmendmentNodeId()
    {
        StartProcessing = true;
        var check = await _amendmentRepository.CheckBeforChangeAmendmentNode(node.Id, AmendmentId);
        if (!check.Flag)
        {
            var response = await _amendmentRepository.ReassignmentAmendment(AmendmentId, node.Id, stateContainerService.user.Id);
            await HandleEventChangeAmendmentNodeId.InvokeAsync(response.Flag);
            await jsRuntime.InvokeVoidAsync(Constants.HideModal, "ModelReassignment");
            AmendmentId = PhaseId = NodeId = Guid.Empty;
        }
        else
        {
            toastService.ShowError("لا يمكن نقل تعديل إلى هذه العقدة .", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
        StartProcessing = false;

    }
}

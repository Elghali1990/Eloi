namespace E.Loi.Components.Pages.Laws;

public partial class BrowsLaw
{
    /*
     * === Global Variables ===
    */
    [Parameter]
    public Guid LawId { get; set; } = Guid.Empty;
    private string LawTitle = string.Empty, NodeContent = string.Empty, NodeLabel = string.Empty, oldContent = string.Empty;
    public List<NodeVm>? nodes { get; set; }
    private bool isLoadToBrowse = false, IsPrintNodeContent = false, DisplayBoxViewNodeContent = false, ShowBtnSetNewNodeContent = false;
    private Guid NodeId = Guid.Empty;
    private string IdPhase = string.Empty;
    List<FlatNode> parentsNode = new();

    /*
     * === OnParametersSetAsync ===
    */
    protected override async Task OnParametersSetAsync()
    {
        if (LawId != Guid.Empty)
        {
            isLoadToBrowse = true;
            var law = await _lawRepository.GetByIdAsync(LawId);
            IdPhase = _phaseOptions.Value.PHASE_COMMISSION_1;
            LawTitle = law!.Label;
            nodes = await _nodeRepository.GetRecursiveChildren(LawId, Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1), false);
            isLoadToBrowse = false;
            await _traceService.insertTrace(new Trace { Operation = "Brows Law ", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
        }
        DisplayBoxViewNodeContent = false;
    }


    /*
     * === Close Canvase ===
    */
    async Task CloseCanvase()
    {
        nodes?.Clear();
        DisplayBoxViewNodeContent = false;
        LawTitle = NodeContent = string.Empty;
        await jsRuntime.InvokeVoidAsync("HideCanvas", "canvasBrowsLaw");
    }

    /*
     * === Get Node Content ===
    */
    async Task getNodeContent(NodeVm node_)
    {
        DisplayBoxViewNodeContent = true;
        parentsNode = (await _nodeRepository.GetFlatParents(node_.Id)).ToList();
        var node = await _nodeRepository.GetNodeContent(node_.Id);
        NodeContent = oldContent = node.NodeContent;
        NodeId = node_.Id;
        NodeLabel = node_.Label!;
        if (IdPhase.ToLower() != _phaseOptions.Value.PHASE_COMMISSION_1.ToLower() && IdPhase.ToLower() != _phaseOptions.Value.PHASE_COMMISSION_2.ToLower())
        {
            ShowBtnSetNewNodeContent = true;
        }
        else
        {
            ShowBtnSetNewNodeContent = false;
        }
    }

    /*
     * === Print Node Content ===
    */
    private async Task PrintNodeContent()
    {
        try
        {
            IsPrintNodeContent = true;
            await _printService.PrintNodeContent(NodeId);
            IsPrintNodeContent = false;
            await _traceService.insertTrace(new Trace { Operation = "Print Node content", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {PrintNodeContent}", nameof(BrowsLaw));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private async Task ChangePhaseLaw(string PhaseId_)
    {
        try
        {
            if (!string.IsNullOrEmpty(PhaseId_))
            {
                isLoadToBrowse = true;
                var law = await _lawRepository.GetByIdAsync(LawId);
                nodes = await _nodeRepository.GetRecursiveChildren(law.Id, Guid.Parse(PhaseId_), false);
                IdPhase = PhaseId_;
                isLoadToBrowse = DisplayBoxViewNodeContent = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"error on {nameof(ChangePhaseLaw)}", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    public async Task SetNewContent()
    {
        try
        {
            var newContent = new UpdateNodeContent()
            {
                NodeId = NodeId,
                NewContent = NodeContent,
                OldContent = oldContent,
                LastModifiedBy = stateContainerService.user.Id
            };
            var response = await _nodeRepository.SetNewContent(newContent);
            if (response.Flag)
            {
                await _traceService.insertTrace(new Trace()
                {
                    UserId = stateContainerService.user.Id,
                    DateOperation = DateTime.UtcNow,
                    Operation = "UPDATE_NODE_CONTENT"
                });
                toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"error on {nameof(SetNewContent)}", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
}

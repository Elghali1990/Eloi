namespace E.Loi.Components.Pages.Utils;

public partial class GetRefeence
{
    List<NodeVm> nodes = new();
    private string RefLawLabel = string.Empty, NodeRefContent = string.Empty;
    [Parameter]
    public Guid refLawId { get; set; }
    [Parameter]
    public EventCallback<string> EventPaseNodeContent { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if (refLawId != Guid.Empty)
        {
            RefLawLabel = string.Empty;
            var law = await _lawRepository.GetByIdAsync(refLawId);
            nodes.Clear();
            RefLawLabel = law.Label;
            Guid phaseId = law.PhaseLawIds.First().PhaseId ?? Guid.Empty;
            nodes = await _nodeRepository.GetRecursiveChildren(refLawId, phaseId, false);
        }
    }

    protected async Task getNodeContent(Guid NodeId)
    {
        try
        {
            var nodeContentVm = await _nodeRepository.GetNodeContent(NodeId);
            if (nodeContentVm is not null)
            {
                NodeRefContent = nodeContentVm.NodeContent;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, $"Error on {nameof(getNodeContent)}", nameof(Amendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private async Task SetTextOriginal()
    {
        await jsRuntime.InvokeVoidAsync(Constants.HideModal, "ModalRefLaw");
        await EventPaseNodeContent.InvokeAsync(NodeRefContent);
    }
}

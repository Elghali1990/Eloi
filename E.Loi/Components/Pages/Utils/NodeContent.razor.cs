namespace E.Loi.Components.Pages.Utils;

public partial class NodeContent
{
    [Parameter] public string content { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> MyEvent { get; set; }
    [Parameter] public Guid NodeId { get; set; } = Guid.Empty;
    public void PassValue() => MyEvent.InvokeAsync(content);
    List<FlatNode> parentsNode = new();
    bool isDownload = false;


    /*
     * === On Parameters Set Async ===
    */
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            if (NodeId != Guid.Empty)
            {
                var parents = await _nodeRepository.GetFlatParents(NodeId);
                parentsNode = parents.ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on OnParametersSetAsync", nameof(NodeContent));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    private async Task handlePrintNodeContent()
    {
        try
        {
            if (NodeId != Guid.Empty)
            {
                isDownload = !isDownload;
                await _printService.PrintNodeContent(NodeId);
                isDownload = !isDownload;
            }
        }
        catch (Exception ex)
        {
            isDownload = false;
            _logger.LogError(ex.Message, "Error on OnParametersSetAsync", nameof(NodeContent));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


}

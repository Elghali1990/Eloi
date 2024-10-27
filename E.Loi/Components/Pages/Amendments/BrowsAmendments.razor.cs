namespace E.Loi.Components.Pages.Amendments;

public partial class BrowsAmendments
{
    /*
 * === Global Variables ===
*/
    [Parameter]
    public Guid LawId { get; set; } = Guid.Empty;
    public List<NodeVm>? nodes { get; set; }
    private bool isLoadToBrowse = false, showAmendmentList = false;
    private Guid NodeId = Guid.Empty, IdNode = Guid.Empty;
    private string IdPhase = string.Empty, LawTitle = string.Empty;
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
            isLoadToBrowse = showAmendmentList = false;
            await _traceService.insertTrace(new Trace { Operation = "Brows Public Amendments ", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });

        }
    }

    /*
     * === Close Canvase ===
    */
    async Task CloseCanvase()
    {
        nodes?.Clear();
        LawTitle = string.Empty;
        showAmendmentList = false;
        await jsRuntime.InvokeVoidAsync(Constants.HideCanvas, "canvasBrowsAmendment");
    }

    /*
     * Display law content by phase
    */
    private async Task ChangePhaseLaw(List<string> PhaseIds)
    {
        try
        {
            if (PhaseIds.Count > 0)
            {
                isLoadToBrowse = true;
                var law = await _lawRepository.GetByIdAsync(LawId);
                nodes = await _nodeRepository.GetRecursiveChildren(law.Id, Guid.Parse(PhaseIds[0]), false);
                IdPhase = PhaseIds[0];
                isLoadToBrowse = showAmendmentList = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"error on {nameof(ChangePhaseLaw)}", nameof(Home));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    private void DisplayAllAmendments(Guid nodeId)
    {
        IdNode = nodeId;
        showAmendmentList = true;
    }

}

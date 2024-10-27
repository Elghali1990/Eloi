namespace E.Loi.Components.Pages.Amendments;

public partial class AmendmentDetail
{
    [Parameter]
    public Guid AmdId { get; set; }
    AmendmentDetails amendmentDetails = new();
    List<FlatNode> parentsNode = new();
    protected override async Task OnParametersSetAsync()
    {
        if (AmdId != Guid.Empty)
        {
            await _traceService.insertTrace(new Trace { Operation = "Get Amendment Details", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            amendmentDetails = await _amendmentRepository.GetAmendmentDetailsAsync(AmdId);
            parentsNode = (await _nodeRepository.GetFlatParents(amendmentDetails.NodeId)).ToList();
        }
    }
}

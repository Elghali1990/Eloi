namespace E.Loi.Components.Pages.Laws;

public partial class EditLawContent
{
    private string? NodeContent, NewContent;
    private List<NodeVm> nodes = new();
    private List<FlatNode> parentsNode = new();
    private List<AmendmentsListVm> amendments = new();
    [Parameter]
    public Guid LawId { get; set; }
    private int count = 0;
    private bool isLoad = false, IsLoadAmendments = true, isSuccess = false, IsContentLoaded = false, startProcessing = false;
    private Guid phaseLawId = Guid.Empty, IdLaw = Guid.Empty;
    /*
    * ===  Get Nodes Of Law ===
    */
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            if (LawId != Guid.Empty)
            {
                isLoad = true;
                IsContentLoaded = false;
                amendments.Clear();
                nodes.Clear();
                var law = await _lawRepository.GetByIdAsync(LawId);
                if (law is null)
                {
                    return;
                }
                if (stateContainerService.user.Roles.Count(r => r.Name == _roleOptions.Value.MEMBER_LEGISLATION) > 0)
                {
                    var phase = law.PhaseLawIds.Where(p => p.PhaseId.ToString().ToLower() == _phaseOptions.Value.PHASE_SEANCE_PLENIERE_1.ToLower() || p.PhaseId.ToString().ToLower() == _phaseOptions.Value.PHASE_SEANCE_PLENIERE_2.ToLower()).ToList();
                    phaseLawId = phase.First(p => p.Statu == PhaseStatu.OPENED.ToString()).PhaseId ?? Guid.Empty;
                }
                else
                {
                    var phase = law.PhaseLawIds.Where(p => p.PhaseId.ToString().ToLower() == _phaseOptions.Value.PHASE_COMMISSION_1.ToLower() || p.PhaseId.ToString().ToLower() == _phaseOptions.Value.PHASE_COMMISSION_2.ToLower()).ToList();
                    phaseLawId = phase.First(p => p.Statu == PhaseStatu.OPENED.ToString()).PhaseId ?? Guid.Empty;
                }
                IdLaw = law.Id;
                nodes = await _nodeRepository.GetRecursiveChildren(law.Id, phaseLawId, true);
                isLoad = false;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, "Error on getRefLawNodes", nameof(Amendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     *=== Get Node Content && Amendment ===
    */
    protected async Task getNodeAmendments(NodeVm Node)
    {
        try
        {
            IsLoadAmendments = false;
            var node = await _nodeRepository.GetNodeContent(NodeId: Node.Id);
            if (node is not null)
            {
                var parents = await _nodeRepository.GetFlatParents(nodeId: node.Id);
                parentsNode = parents.ToList();
                NodeContent = node.NodeContent;
                amendments = await _amendmentRepository.GetNodeAmendmentsAsync(NodeId: Node.Id);

                NewContent = !string.IsNullOrEmpty(amendments.First().NewContent) ? amendments.First().NewContent : node.NodeContent;

            }
            IsLoadAmendments = true;
            IsContentLoaded = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, "Error on getNodeAmendments", nameof(FollowLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    private void HideAlert() => isSuccess = false;

    /*
     * === Set New Content For Amendment ===
    */
    protected async Task SetAmendmentContent()
    {
        try
        {
            if (amendments.Count > 0)
            {
                startProcessing = true;
                var model = new SetContent()
                {
                    Ids = amendments.Select(x => x.Id).ToList(),
                    Content = NewContent!
                };
                var response = await _amendmentRepository.SetNewContent(model);
                isSuccess = response.Flag;
                if (response.Flag)
                {
                    toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    await _traceService.insertTrace(new Trace { Operation = "Set AmendmentContent", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                    nodes = await _nodeRepository.GetRecursiveChildren(IdLaw, phaseLawId, true);
                }
                else
                {
                    toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
                IsContentLoaded = false;
                NodeContent = NewContent = string.Empty;
            }
            startProcessing = false;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, "Error on SetAmendmentContent", nameof(FollowLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            startProcessing = false;
        }
    }
}

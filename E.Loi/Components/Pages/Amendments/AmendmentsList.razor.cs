using Microsoft.AspNetCore.WebUtilities;

namespace E.Loi.Components.Pages.Amendments;

public partial class AmendmentsList
{
    /*
     * === Variabls Region ===
    */
    [Parameter] public EventCallback<string> HandleEventEditAmendment { get; set; }
    [Parameter] public string amendmentId { get; set; } = string.Empty;
    [Parameter] public Guid PhaseId { get; set; } = Guid.Empty;
    [Parameter] public Guid NodeId { get; set; } = Guid.Empty;
    List<AmendmentsListVm> amendmentsListVms = new();
    List<AmendmentsListVm> amendmentsListVms_ = new();
    List<FlatNode> parentsNode = new();
    List<TeamVm> teams = new();
    private SearchVm search = new();
    public Guid AmendId = Guid.Empty, IdNode = Guid.Empty;
    public Node? node;
    List<int> pages = new();
    private int totalItems, currentPage, TotalPages = 0;
    private bool checkAll = false, checkboxCheckAll = false, isDownload = true, isLoad = true, startSetAmendmentNumber = false;
    private List<Guid> AmdIds = new();


    /*
     * === On Parameters Set Async ===
    */
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            if (NodeId != Guid.Empty)
            {
                isLoad = false;
                parentsNode = (await _nodeRepository.GetFlatParents(NodeId)).ToList();
                amendmentsListVms = await _amendmentRepository.GetAmendmentsListAsync(stateContainerService.user.TeamsDtos!.Select(t => t.Id).ToList(), NodeId);
                amendmentsListVms_ = amendmentsListVms;
                TotalPages = (int)Math.Ceiling((double)amendmentsListVms.Count / Constants.pageSize);
                amendmentsListVms = amendmentsListVms.Skip((currentPage - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
                isLoad = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on Get Amendments List", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === OnInitializedAsync ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            teams = await _teamRepository.GetAllTeamsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error {nameof(OnInitializedAsync)}", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Delete Amendment ===
    */
    private async Task HandleDeleteAmendment(Guid Id)
    {
        try
        {
            var response = await _amendmentRepository.DeleteAmendmantAsync(Id, stateContainerService.user!.Id);
            if (response.Flag)
            {
                toastService.ShowSuccess(Constants.MessageSuccessDeleteAmandment, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                amendmentsListVms = await _amendmentRepository.GetAmendmentsListAsync(stateContainerService.user!.TeamsDtos!.Select(t => t.Id).ToList(), NodeId);
                amendmentsListVms_ = amendmentsListVms;
                StateHasChanged();
                await _traceService.insertTrace(new Trace { Operation = "Delete Amendments", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${HandleDeleteAmendment}", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Edit Amendment ===
    */
    protected async Task HandleEditAmendment(string idAmendment)
    {
        stateContainerService.state = new() { ShowViewListAmendments = false, ShowAddAmendment = true, ShowViewNodeContent = false };
        await HandleEventEditAmendment.InvokeAsync(amendmentId = idAmendment.ToString());
    }

    /*
     * === Close Amendment ===
    */
    protected async Task HandleCloseAmendments()
    {
        try
        {
            if (AmdIds.Count > 0)
            {
                if (amendmentsListVms.Any(a => AmdIds.Any(id => id == a.Id) && a.AmendmentsStatus == Constants.Public))
                {
                    toastService.ShowError(Constants.MessageErrorSetAmendmentStatu, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
                else
                {
                    if (await setAmendmentsStatu(AmendmentsStatus.FINAL.ToString()))
                    {
                        toastService.ShowSuccess(Constants.MessageSuccessCloseAmendment, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                        await _traceService.insertTrace(new Trace { Operation = "Close Amendments", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                    }
                    else
                    {
                        toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    }
                }
                AmdIds.Clear();
            }
            else
            {
                toastService.ShowError("المرجو إختيار التعديلات المراد إغلاقها.", settings => { settings.DisableTimeout = settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "error on closeAmendment", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Set Amendment Statut Method ===
    */
    private async Task<bool> setAmendmentsStatu(string statu)
    {
        try
        {
            SetAmendmentStatuVm vm = new()
            {
                Ids = AmdIds,
                AmendmentStatu = statu,
                UserId = stateContainerService.user!.Id
            };
            var response = await _amendmentRepository.SetAmendmentsAsync(vm);
            if (response.Flag)
            {
                foreach (var Id in AmdIds)
                {
                    var amendment = amendmentsListVms!.FirstOrDefault(a => a.Id == Id);
                    if (amendment is not null) amendment.AmendmentsStatus = Constants.GetAmendmentStatu(statu);
                    if (statu == AmendmentsStatus.SUBMITTED.ToString())
                    {
                        amendment!.SubmitedDate = DateTime.UtcNow;
                    }
                    StateHasChanged();
                }
            }
            return response.Flag;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${nameof(setAmendmentsStatu)}", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            return false;
        }
    }

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
            if (AmdIds.Count > 0)
            {
                isDownload = false;
                var url = _navigationManager.Uri.Split('/');
                var law = await _lawRepository.GetByIdAsync(Guid.Parse(url[url.Length - 1]));
                SetAmendData amendData = new();
                amendData.lawNumber = law.Number;
                amendData.lawYear = law.Year;
                amendData.OutType = OutType;
                amendData.TeamId = stateContainerService.user!.TeamId;
                amendData.WhithVote = false;
                amendData.amendmentsIds = AmdIds;
                var bytes = await _editionRepository.PrintTeamAmendments(amendData);
                if (bytes is not null)
                {
                    await _printService.DownloadAmendments(bytes, stateContainerService.user.TeamName, OutType);
                }
                string message = OutType == "docx" ? "Print Amendments Word" : "Print Amendments Pdf";
                await _traceService.insertTrace(new Trace { Operation = message, DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                isDownload = true;
            }
            else
            {
                toastService.ShowError("المرجو إختيار التعديلات المراد تحميلها.", settings => { settings.DisableTimeout = settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error {nameof(PrintAmendments)}", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Get Amendment Detail ===
    */
    protected async Task handleGetAmendmentDetails(Guid Id)
    {
        AmendId = Id;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModalDetail");
    }

    /*
     * === Open Amendment ===
    */
    protected async Task HandleOpenAmendments()
    {
        try
        {
            if (AmdIds.Count > 0)
            {
                if (amendmentsListVms.Any(a => AmdIds.Any(id => id == a.Id) && a.AmendmentsStatus == Constants.Public))
                {
                    toastService.ShowError(Constants.MessageErrorSetAmendmentStatu, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
                else
                {
                    if (await setAmendmentsStatu(AmendmentsStatus.EDITABLE.ToString()))
                    {
                        toastService.ShowSuccess(Constants.MessageSuccessOpenAmendment, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                        await _traceService.insertTrace(new Trace { Operation = "Open Amendments", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                    }
                    else
                    {
                        toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    }
                }
                AmdIds.Clear();
            }
            else
            {
                toastService.ShowError("المرجو إختيار التعديلات المراد فتحها.", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "error on closeAmendment", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    /*
     * === Submit Amendment To Commission ===
    */
    protected async Task HandleSubmitAmendments()
    {
        try
        {
            if (AmdIds.Count > 0)
            {
                if (amendmentsListVms.Any(a => AmdIds.Any(id => id == a.Id) && a.AmendmentsStatus == Constants.Public))
                {
                    toastService.ShowError(Constants.MessageErrorSetAmendmentStatu, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
                else
                {
                    if (await setAmendmentsStatu(AmendmentsStatus.SUBMITTED.ToString()))
                    {
                        toastService.ShowSuccess(Constants.MessageSuccessSubmitAmendment, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                        await _traceService.insertTrace(new Trace { Operation = "Submit Amendments", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                    }
                    else
                    {
                        toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    }
                }
                AmdIds.Clear();
            }
            else
            {
                toastService.ShowError("المرجو إختيار التعديلات المراد إحالتها.", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "error on closeAmendment", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Publich Amendments  Commission ===
    */
    protected async Task HandlPublichCommAmendments()
    {
        try
        {
            if (await setAmendmentsStatu(AmendmentsStatus.PUBLIC.ToString()))
                toastService.ShowSuccess(Constants.MessageSuccessPublishAmendment, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            else
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "error on closeAmendment", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Filter Amendments List ===
    */
    private void Filter(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            amendmentsListVms = amendmentsListVms_;
        else
        {
            amendmentsListVms.Clear();
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
    }

    public async Task handleReassignmentAmendment(Guid IdAmendment, Guid NodeId)
    {
        AmendId = IdAmendment;
        IdNode = NodeId;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModelReassignment");
    }

    public async Task SelectedNode(Guid nodeId)
    {
        node = await _nodeRepository.GetNodeByID(nodeId);
    }
    /*
     * === Pagination ===
   */


    private void NextPage()
    {
        currentPage++;
        amendmentsListVms = amendmentsListVms_.Skip((currentPage - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
    }
    private void PreviousPage()
    {
        currentPage--;
        amendmentsListVms = amendmentsListVms_.Skip((currentPage - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
    }

    /*
     * === Select All Amendments ===
    */
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

    /*
     * === Select Amendment === 
    */
    private void CheckAmendment(ChangeEventArgs e, Guid Id)
    {
        if ((bool)e.Value! == true)
        {
            checkAll = false;
            AmdIds.Add(Id);
            if (AmdIds.Count == amendmentsListVms.Count)
                checkboxCheckAll = true;
        }
        else
        {
            AmdIds.Remove(Id);
            checkboxCheckAll = false;
        }
    }

    /*
     *=== Handle change team
    */

    private async Task HandleChangeTeam()
    {
        if (AmdIds.Count == 0)
        {
            toastService.ShowError("المرجو تحديد التعديلات .", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
        else
        {
            await jsRuntime.InvokeVoidAsync(Constants.ShowModal, "ModalChangeTeam");
        }
    }

    private async Task EventChangeTeam(bool response)
    {
        await jsRuntime.InvokeVoidAsync(Constants.HideModal, "ModalChangeTeam");
        if (response)
        {
            toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            isLoad = false;
            amendmentsListVms=amendmentsListVms_ = await _amendmentRepository.GetAmendmentsListAsync(stateContainerService.user.TeamsDtos!.Select(t => t.Id).ToList(), NodeId);
            isLoad = true;
        }
        else
        {
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    /*
     * === Set Amendments Number ===
    */

    private async Task SetAmendmentsNumber()
    {
        try
        {
            startSetAmendmentNumber = true;
            var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);
            string LawId = uri.Segments[uri.Segments.Length - 1];
            var TeamIds = stateContainerService.user.TeamsDtos!.Select(t => t.Id).ToList();
            var response = await _amendmentRepository.SetAmendmentsNumbers(TeamIds, Guid.Parse(LawId), PhaseId, stateContainerService.user.Id);
            if (response.Flag)
            {
                amendmentsListVms = amendmentsListVms_ = await _amendmentRepository.GetAmendmentsListAsync(stateContainerService.user.TeamsDtos!.Select(t => t.Id).ToList(), NodeId);
                toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            startSetAmendmentNumber = false;
        }
        catch (Exception ex)
        {
            startSetAmendmentNumber = false;
            _logger.LogError(ex.Message, $"error on {nameof(SetAmendmentsNumber)}", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    public async Task EventChangeAmendmentNodeId(bool isSuccess)
    {
        await jsRuntime.InvokeVoidAsync(Constants.HideModal, "ModelReassignment");
        if (isSuccess)
        {
            amendmentsListVms = await _amendmentRepository.GetAmendmentsListAsync(stateContainerService.user.TeamsDtos!.Select(t => t.Id).ToList(), NodeId);
            amendmentsListVms_ = amendmentsListVms;
            toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
        else
        {
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
}


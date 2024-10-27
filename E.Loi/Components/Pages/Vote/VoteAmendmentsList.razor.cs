namespace E.Loi.Components.Pages.Vote;

public partial class VoteAmendmentsList
{

    /*
     * === Globale Variables ===
     */
    [Parameter] public Guid NodeId { get; set; } = Guid.Empty;
    [Parameter] public Guid PhaseId { get; set; } = Guid.Empty;
    [SupplyParameterFromForm]
    private VoteVm voteVm { get; set; } = new();
    List<AmendmentsListVm> amendments = new();
    List<AmendmentsListVm> _amendments = new();
    bool isLoad = false, checkboxCheckAll = false, checkAll = false, IsPrint;
    List<FlatNode> parentsNode = new();
    AmendmentDetails amendmentDetails = new();
    Guid AmendmentId = Guid.Empty;
    List<TeamVm> teams = new();
    private List<Guid> amendentsIds = new();
    [Parameter]
    public EventCallback<bool> onVoteAmendment { get; set; }
    [Parameter]
    public EventCallback<bool> onDeleteVoteAmendment { get; set; }
    List<int> pages = new();
    private int totalItems, currentPage, TotalPages = 0;
    /*
     * === Set Parametres of page === 
    */
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            if (NodeId != Guid.Empty)
            {
                isLoad = true;
                amendments = _amendments = (await _amendmentRepository.GetAmendmentsListForVotingAsync(NodeId)).OrderBy(amd => amd.Order).ToList();
                TotalPages = (int)Math.Ceiling((double)_amendments.Count / Constants.pageSize);
                amendments = _amendments.Skip((currentPage - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
                parentsNode = (await _nodeRepository.GetFlatParents(NodeId)).ToList();
                teams = await _teamRepository.GetAllTeamsAsync();
                isLoad = checkAll = checkboxCheckAll = false;
                amendentsIds.Clear();
                FillListOfPage(Constants.getPageSize(_amendments.Count));
            }
            else
            {
                amendments = _amendments = null!;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on OnParametersSetAsync", nameof(VoteAmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    void FillListOfPage(int count)
    {
        pages.Clear();
        for (int i = 1; i <= count; i++)
        {
            pages.Add(i);
        }
    }

    /*
     * === Show Modal Vote ===
    */
    protected async Task ShowModalVote() => await jsRuntime.InvokeVoidAsync(Constants.ShowModal, "ModalVote");

    /*
     * === Delete Vote === 
    */
    protected async Task DeleteVoteAsync()
    {
        try
        {
            if (amendentsIds != null)
            {
                DeleteVoteVm vote = new();
                vote.Ids = amendentsIds;
                var response = await _voteAmendmentRepository.DeleteVoteAsync(vote);
                if (response.Flag)
                {
                    await onDeleteVoteAmendment.InvokeAsync(response.Flag);
                    toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    await _traceService.insertTrace(new Trace { Operation = "Remove Vote Amendments", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                    _navigationManager.NavigateTo(_navigationManager.Uri);
                }
                else
                {
                    toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
                amendentsIds.Clear();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on DeleteVoteAsync", nameof(VoteAmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Display Modal Details === 
    */
    protected async Task handleGetAmendmentDetails(Guid Id)
    {
        AmendmentId = Id;
        await jsRuntime.InvokeVoidAsync(Constants.ShowModal, "ModalDetail");
    }

    /*
     * === Print Amendment Pdf === 
    */
    private async Task Print_PDF(bool includeAmendmentRatraper) => await PrintAmendments("pdf", includeAmendmentRatraper);


    private async Task Print_Vote_Amendment_Result()
    {
        if (PhaseId != Guid.Empty)
        {
            IsPrint = true;
            var url = _navigationManager.Uri.Split('/');
            var bytes = await _editionRepository.printVoteAmendmentsResult(Guid.Parse(url[url.Length - 1]), PhaseId, "docx");
            await _printService.setStream(new MemoryStream(bytes), "نتائج التصويت.docx");
            IsPrint = false;
        }
    }
    /*
     * === Print Amendments Word === 
    */
    private async Task Print_WORD(bool includeAmendmentRatraper) => await PrintAmendments("docx", includeAmendmentRatraper);

    /*
     * === Get Document From Stream Of Bytes === 
    */
    private async Task PrintAmendments(string OutType, bool includeAmendmentRatraper)
    {
        try
        {
            IsPrint = true;
            var url = _navigationManager.Uri.Split('/');
            var law = await _lawRepository.GetByIdAsync(Guid.Parse(url[url.Length - 1]));
            Guid PhaseId = law.PhaseLawIds.FirstOrDefault(p => p.PhaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1) || p.PhaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_2) && p.Statu == PhaseStatu.OPENED.ToString()).PhaseId ?? Guid.Empty;
            var bytes = await _editionRepository.printVotingFileCommission(law.Id, PhaseId, OutType, includeAmendmentRatraper);
            if (bytes is not null)
            {
                await _printService.DownloadAmendments(bytes, stateContainerService.user.TeamName, OutType);
            }
            IsPrint = false;
            await _traceService.insertTrace(new Trace { Operation = "Print Vote Result Amendments " + OutType, DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error {nameof(PrintAmendments)}", nameof(VoteAmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Update Data After Insert Vote Amendments === 
    */
    public async Task UpdatingDataAfterVote(bool isSuccess)
    {
        await _traceService.insertTrace(new Trace { Operation = "Vote Amendments ", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
        await onVoteAmendment.InvokeAsync(isSuccess);
    }

    private void Filter(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            amendments = _amendments;
        else
        {
            amendments.Clear();
            if (filterBy == "NumberSystem")
                amendments = _amendments.Where(l => l.NumberSystem.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Number")
                amendments = _amendments.Where(l => l.Number.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Order")
                amendments = _amendments.Where(l => l.Order.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "NodeTitle")
                amendments = _amendments.Where(l => l.NodeTitle!.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Title")
                amendments = _amendments.Where(l => l.Title!.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "AmendmentType")
                amendments = _amendments.Where(l => l.AmendmentIntent!.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "AmendmentsStatus")
                amendments = _amendments.Where(l => l.AmendmentsStatus!.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Team")
                amendments = _amendments.Where(l => l.Team! == e.Value!.ToString()!).ToList();
            if (filterBy == "VoteResult")
                amendments = _amendments.Where(l => l.VoteResult == e.Value!.ToString()!).ToList();
            // amendments = amendments.Skip((currentPage - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
        }
    }
    /*
     * === Select All Amendments ===
    */
    private void CheckAllAmendments()
    {
        amendentsIds.Clear();
        if (checkAll)
        {
            checkAll = checkboxCheckAll = false;
        }
        else
        {
            amendentsIds = _amendments.Select(amd => amd.Id).ToList();
            checkAll = checkboxCheckAll = true;
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
            amendentsIds.Add(Id);
        }
        else
        {
            amendentsIds.Remove(Id);
            checkboxCheckAll = false;
        }
    }

    /*
  * === Pagination ===
*/
    private void NextPage()
    {
        currentPage++;
        amendments = _amendments.Skip((currentPage - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
    }
    private void PreviousPage()
    {
        currentPage--;
        amendments = _amendments.Skip((currentPage - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
    }


    public async Task GenerateExcelFile()
    {
        try
        {
            IsPrint = true;
            byte[] bytes = _excelService.GenerateExcelFile(_amendments);
            await _printService.setStream(new MemoryStream(bytes), "لائحة التعديلات.xlsx");
            IsPrint = false;
        }
        catch (Exception ex)
        {
            Helpers.Trace.Trace.Logging(ex, nameof(GenerateExcelFile), $"{nameof(VoteAmendment)} Component");
            IsPrint = false;
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

}

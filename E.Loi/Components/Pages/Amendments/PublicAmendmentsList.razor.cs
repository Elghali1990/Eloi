namespace E.Loi.Components.Pages.Amendments;

public partial class PublicAmendmentsList
{

    #region === Global Variables ===
    [Parameter] public Guid NodeId { get; set; } = Guid.Empty;
    List<AmendmentsListVm> amendmentsListVms = new();
    List<AmendmentsListVm> amendmentsListVms_ = new();
    bool isDownload = true, isLoad = true;
    List<TeamVm> teams = new();
    List<FlatNode> parentsNode = new();
    Guid AmendmentId = Guid.Empty;
    List<int> pages = new();
    private int curentIndex = 0;
    private bool checkAll = false, checkboxCheckAll = false;
    private List<Guid> AmdIds = new();
    #endregion

    #region === On Parameters Set Async ===
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            if (NodeId != Guid.Empty)
            {
                isLoad = false;
                var parents = await _nodeRepository.GetFlatParents(NodeId);
                parentsNode = parents.ToList();
                amendmentsListVms = await _amendmentRepository.GetPublicAmendmentsListAsync(NodeId);
                amendmentsListVms_ = amendmentsListVms;
                FillListOfPage(Constants.getPageSize(amendmentsListVms.Count));
                isLoad = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on Get public Amendments List", nameof(PublicAmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    #endregion

    protected override async Task OnInitializedAsync()
    {
        teams = await _teamRepository.GetAllTeamsAsync();
    }

    #region === Handle Get Amendment Detail ===
    protected async Task handleGetAmendmentDetails(Guid Id)
    {
        AmendmentId = Id;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModalDetail");
    }
    #endregion

    /*
     * === Print Amendments ===
    */
    private async Task Print_PDF() => await PrintAmendments("pdf");
    private async Task Print_WORD() => await PrintAmendments("docx");

    private async Task PrintAmendments(string OutType)
    {
        try
        {
            if (AmdIds.Count > 0)
            {
                isDownload = false;
                var node = await _nodeRepository.GetNodeByIdAsync(NodeId);
                var law = await _lawRepository.GetByIdAsync(node.LawId);
                SetAmendData amendData = new();
                amendData.lawNumber = law.Number;
                amendData.lawYear = law.Year;
                amendData.OutType = OutType;
                amendData.amendmentsIds = AmdIds;
                var bytes = await _editionRepository.PrintTeamAmendments(amendData);
                if (bytes is not null)
                    await _printService.DownloadAmendments(bytes, law.Label, law.Year, law.Number, OutType);
                isDownload = true;
            }
            else
            {
                toastService.ShowError("المرجو إختيار التعديلات المراد تحميلها.", settings => { settings.DisableTimeout = settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error On Get amendment file stream", nameof(AmendmentsList));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Pagination ===
     */
    void FillListOfPage(int count)
    {
        pages.Clear();
        for (int i = 1; i <= count; i++)
            pages.Add(i);
    }

    private void firstPage()
    {
        amendmentsListVms = amendmentsListVms_;
        amendmentsListVms = amendmentsListVms.Skip(0).Take(Constants.pageSize * 1).ToList();
    }
    private void lastPage()
    {
        amendmentsListVms = amendmentsListVms_.Skip((pages.Last() - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
    }

    private void paginate(int page)
    {

        if (page > curentIndex)
        {
            amendmentsListVms = amendmentsListVms_.Skip((page - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
        }
        else
        {
            amendmentsListVms = amendmentsListVms_;
            amendmentsListVms = amendmentsListVms.Skip(page - 1).Take(Constants.pageSize * (page - 1 == 0 ? 1 : page - 1)).ToList();
        }
        curentIndex = page;
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
        FillListOfPage(Constants.getPageSize(amendmentsListVms.Count));
    }

    /*
     * === Select All Amendments ===
    */
    private void CheckAllAmendments()
    {
        if (checkAll)
        {
            AmdIds.Clear();
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
            {
                checkboxCheckAll = true;
            }
        }
        else
        {
            AmdIds.Remove(Id);
            checkboxCheckAll = false;
        }
        Console.WriteLine(checkboxCheckAll);
    }
}

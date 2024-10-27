namespace E.Loi.Components.Pages.SubmitedLaws;

public partial class TextSubmitedToChamberAdvisors
{
    List<LawVm> laws = new();
    List<LawVm> laws_ = new();
    List<TeamVm> teams = new();
    List<TeamVm> commissions = new();
    private DateTime? dateIhala = null;
    private Guid LawId = Guid.Empty;
    private bool IsLoad = false;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsLoad = true;
            teams = await _teamRepository.GetAllTeamsAsync();
            commissions = await _teamRepository.GetCommissionsAsync();
            laws = laws_ = await _lawRepository.getLawsForReadTwo(Guid.Parse(_phaseOptions.Value.STATU_SUBMITED_To_CHAMBRE_ADVISORS));
            IsLoad = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(createRootNodeAndSetLawStatu)}", nameof(TextSubmitedToChamberAdvisors));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private void Filter(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            laws = laws_;
        else
        {
            if (filterBy == "Label")
                laws = laws_.Where(l => l.Label.Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Year")
                laws = laws.Where(l => l.Year.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "Number")
                laws = laws_.Where(l => l.Number.ToString().Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "TeamName")
                laws = laws_.Where(l => l.TeamName.Contains(e.Value!.ToString()!)).ToList();
            if (filterBy == "CommissionName")
                laws = laws_.Where(l => l.CommissionName == e.Value!.ToString()).ToList();
        }
        //  FillListOfPage(Constants.getPageSize(laws.Count));
    }

    private async Task ShowPopup(Guid lawId)
    {
        LawId = lawId;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModalSetDateIhala");
    }

    private async Task createRootNodeAndSetLawStatu()
    {
        try
        {
            if (dateIhala is not null)
            {
                var law = await _lawRepository.GetByIdAsync(LawId);
                var statu = await _statuRepository.getStatusById((Guid)law.StatuId!);
                if (statu is not null)
                {
                    var statu_ = await _statuRepository.getStatusByOrder((int)statu.Order! + 1);
                    LawStatuVm lawStatu = new()
                    {
                        LawId = LawId,
                        StatuLaw = statu_.Id,
                        DateSetStatu = DateTime.UtcNow,
                        LastModifiedBy = stateContainerService.user.Id
                    };
                    var response = await _lawRepository.SetLawStatuAsync(lawStatu);
                }
                var resp = await _lawRepository.SetPhaseLawAsync(LawId, Guid.Parse(_phaseOptions.Value.PHASE_AFFECTATION_BUREUA_2), stateContainerService.user.Id);
                LawDate lawDate = new()
                {
                    LawId = LawId,
                    LastModifiedBy = stateContainerService.user.Id,
                    DateModification = dateIhala ?? DateTime.UtcNow,
                };
                var response_ = await _lawRepository.setDateIhalaLaw(lawDate);
                CreateNodeVm node = new();
                node.Label = law.Label;
                node.TypeId = Guid.Parse("665AF4BD-DECA-47CC-7DD1-08DA3CE94CE8");
                node.Content = string.Empty;
                node.OriginalContent = string.Empty;
                node.PhaseLawId = Guid.Parse(_phaseOptions.Value.PHASE_AFFECTATION_BUREUA_2);
                node.LawId = LawId;
                node.ParentNodeId = null;
                node.CreatedBy = stateContainerService.user!.Id;
                var resutl = await _nodeRepository.CreateNode(node);
                await jsRuntime.InvokeVoidAsync("HideModal", "ModalSetDateIhala");
                toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                laws = laws_ = await _lawRepository.getLawsForReadTwo(Guid.Parse(_phaseOptions.Value.STATU_SUBMITED_To_CHAMBRE_ADVISORS));
                await _traceService.insertTrace(new Trace { Operation = "Add Law Read Two", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(createRootNodeAndSetLawStatu)}", nameof(TextSubmitedToChamberAdvisors));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
}

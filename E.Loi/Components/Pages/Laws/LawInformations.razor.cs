namespace E.Loi.Components.Pages.Laws;

public partial class LawInformations
{
    private LawInfo lawInfo = new();
    List<Legislative> legislatives = new();
    List<LegislativeYear> years = new();
    List<LegislativeSession> sessions = new();
    string? legislative, year;
    private bool isLoad = false, IS_MEMBER_SEANCE = false, IS_MEMBER_COMMISSION = false, IS_MEMBER_LEGISLATION = false;
    List<TeamVm> commissions = new();
    List<TeamVm> teams = new();
    List<Statut> status = new();
    [Parameter]
    public Guid LawId { get; set; }
    [Parameter]
    public EventCallback<bool> OnSetLawInfo { get; set; }
    public List<string> errors = new();
    private DateTime FirstTime { get; set; }
    private DateTime SecondTime { get; set; }
    protected override async Task OnParametersSetAsync()
    {

        try
        {
            if (LawId != Guid.Empty)
            {
                Guid phaseId = Guid.Empty;
                commissions.Clear();
                IS_MEMBER_SEANCE = !stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_SEANCE) || r.Name.Equals(_roleOptions.Value.DIRECTEUR_LEGISLATION));
                IS_MEMBER_COMMISSION = !stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_COMMISSION) || r.Name.Equals(_roleOptions.Value.DIRECTEUR_LEGISLATION));
                IS_MEMBER_LEGISLATION = !stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_LEGISLATION) || r.Name.Equals(_roleOptions.Value.DIRECTEUR_LEGISLATION));
                legislatives = await _legislativeRepository.getAll();
                commissions = await _teamRepository.GetCommissionsAsync();
                teams = await _teamRepository.GetAll();
                lawInfo = await _lawRepository.GetLawInfoAsync(LawId);
                years = await _legislativeYearsRepository.GetLegislativeYearsByIdLegislative(lawInfo.LegId.ToString());
                sessions = await _legislativeSessionsRepository.GetAllByIdYear(lawInfo.YearId.ToString());
                status.Clear();
                var response = await _lawRepository.GetLawStatuAsync(LawId);
                if (response.Flag)
                {
                    int order = int.Parse(response.Massage) + 1;
                    var law = await _lawRepository.GetByIdAsync(LawId);
                    phaseId = law.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Empty;
                    if (law.IdLegislative == null)
                    {
                        lawInfo.LegId = legislatives.Last().Id;
                        years = await _legislativeYearsRepository.GetLegislativeYearsByIdLegislative(lawInfo.LegId.ToString());
                        lawInfo.YearId = years.Last().Id;
                        sessions = await _legislativeSessionsRepository.GetAllByIdYear(lawInfo.YearId.ToString());
                        lawInfo.SessionId = sessions.Last().Id;
                    }
                    if (stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_COMMISSION)))
                    {
                        if (law.PhaseLawIds.Count(p => p.Statu == PhaseStatu.OPENED.ToString() && p.PhaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1)) > 0)
                        {
                            phaseId = Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1);
                        }
                        if (law.PhaseLawIds.Count(p => p.Statu == PhaseStatu.OPENED.ToString() && p.PhaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_2)) > 0)
                        {
                            phaseId = Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_2);
                        }
                        FirstTime = lawInfo.DateFinAmendments1 ?? DateTime.Now.Date;
                        SecondTime = lawInfo.DateFinAmendments2 ?? DateTime.Now.Date;
                    }
                    else if (stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_SEANCE)))
                    {
                        if (law.PhaseLawIds.Count(p => p.Statu == PhaseStatu.OPENED.ToString() && p.PhaseId == Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1)) > 0)
                        {
                            phaseId = Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1);
                        }
                        if (law.PhaseLawIds.Count(p => p.Statu == PhaseStatu.OPENED.ToString() && p.PhaseId == Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_2)) > 0)
                        {
                            phaseId = Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_2);
                        }
                    }
                    status = await _statuRepository.getStatusByPhaseId(phaseId, order);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(HandleSetLawInfo)}", nameof(FollowLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    /*
    * === Bind List Years ===
    * 
    */
    private async Task selectLegislative(ChangeEventArgs e)
    {
        try
        {
            isLoad = true;
            years.Clear();
            sessions.Clear();
            legislative = e.Value!.ToString();
            lawInfo.LegId = Guid.Parse(legislative!);
            years = await _legislativeYearsRepository.GetLegislativeYearsByIdLegislative(legislative!);
            isLoad = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on selectLegislative", nameof(FollowLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
    * === Bind List Sessions ===
    */
    private async Task selectYear(ChangeEventArgs e)
    {
        try
        {
            isLoad = true;
            sessions.Clear();
            year = e.Value!.ToString();
            lawInfo.YearId = Guid.Parse(year!);
            sessions = await _legislativeSessionsRepository.GetAllByIdYear(year!);
            isLoad = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, "Error on selectYear", nameof(FollowLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    public async Task HandleSetLawInfo()
    {
        try
        {
            lawInfo.UserId = stateContainerService.user!.Id;
            lawInfo.LawId = LawId;
            if (stateContainerService.user.Roles!.Any(role => role.Name.Equals(_roleOptions.Value.MEMBER_COMMISSION)))
            {
                if (lawInfo.DateFinAmendments1 != null)
                {

                    var date = lawInfo.DateFinAmendments1?.ToString("yyyy-MM-dd");
                    lawInfo.DateFinAmendments1 = Convert.ToDateTime(date).Add(new TimeSpan(FirstTime.Hour, FirstTime.Minute, 0));
                }
                if (lawInfo.DateFinAmendments2 != null)
                {

                    var date = lawInfo.DateFinAmendments2?.ToString("dd-MM-YYYY");
                    lawInfo.DateFinAmendments2 = Convert.ToDateTime(date).Add(new TimeSpan(SecondTime.Hour, SecondTime.Minute, 0));
                }
            }
            if (MessageErrors(lawInfo).Count() == 0)
            {
                var response = await _lawRepository.SetLawInfo(lawInfo);
                LawId = Guid.Empty;
                await jsRuntime.InvokeVoidAsync("HideCanvas", "offcanvasTopEditLawInfo");
                await OnSetLawInfo.InvokeAsync(response.Flag);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(HandleSetLawInfo)}", nameof(FollowLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private async Task closeModel()
    {
        await jsRuntime.InvokeVoidAsync("HideCanvas", "offcanvasTopEditLawInfo");
    }

    private List<string> MessageErrors(LawInfo lawInfo)
    {
        errors.Clear();
        if (lawInfo.LegId == Guid.Empty)
            errors.Add("المرجو إختيار الولاية التشريعية");
        if (lawInfo.YearId == Guid.Empty)
            errors.Add("المرجو إختيار السنة التشريعية");
        if (lawInfo.SessionId == Guid.Empty)
            errors.Add("المرجو إختيار الدورة التشريعية");
        if (lawInfo.LawLabel == string.Empty || lawInfo.LawLabel == null)
            errors.Add("المرجو إدخال عنوان النص .");
        if (lawInfo.IdTeam == Guid.Empty)
            errors.Add("المرجو إختيار صاحب النص");
        if (lawInfo.DateAffectationBureau == null)
            errors.Add("المرجو إدخال تاريخ إحالة على مكتب المجلس.");
        if (lawInfo.CommissionId == Guid.Empty)
            errors.Add("المرجو إختيار اللجنة .");
        if (lawInfo.DateAffectationCommission1 == null)
            errors.Add("المرجو إدخال تاريخ إحالة على اللجنة.");
        return errors;
    }

}

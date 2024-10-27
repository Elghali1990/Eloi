namespace E.Loi.Components.Pages.Laws;
public partial class FollowLaws
{

    /*
     *=== Global Variables For Component === 
    */
    private bool isLoad = false, IS_MEMBER_SEANCE = false, IS_MEMBER_COMMISSION = false, IS_MEMBER_LEGISLATION = false, IsLoadLawInfo = false, startCloneSection = false, isClone, startSubmitedText = false;
    List<LawVm> laws = new();
    List<LawVm> laws_ = new();
    int curentIndex = 0, pageSize = 10;
    List<TeamVm> commissionsFilter = new();
    List<TeamVm> teams = new();
    List<Phase> phases = new();
    public FlatNode[] nodeSection = new FlatNode[0];
    private Guid LawId = Guid.Empty, LawInformationId = Guid.Empty, BrowsLawId = Guid.Empty, EditlawContentId = Guid.Empty, sectionId = Guid.Empty, SectionLawId = Guid.Empty;
    List<int> pages = new();
    List<Statut> status = new();

    /*
     * === On Component Load === 
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoad = true;
            IS_MEMBER_SEANCE = !stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_SEANCE) || r.Name.Equals(_roleOptions.Value.DIRECTEUR_LEGISLATION));
            IS_MEMBER_COMMISSION = !stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_COMMISSION) || r.Name.Equals(_roleOptions.Value.DIRECTEUR_LEGISLATION));
            IS_MEMBER_LEGISLATION = !stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_LEGISLATION) || r.Name.Equals(_roleOptions.Value.DIRECTEUR_LEGISLATION));
            var userString = await _sessionStorageService.GetItemAsStringAsync("User");
            if (!string.IsNullOrEmpty(userString))
            {
                var user = JsonConvert.DeserializeObject<UserVm>(userString);
                stateContainerService.user = new() { UserName = user!.FullName, Roles = user.Roles };
                stateContainerService.user = user;
            }
            await LoadLaws();
            commissionsFilter = await _teamRepository.GetCommissionsAsync();
            teams = await _teamRepository.GetAllTeamsAsync();
            status = (await _statuRepository.getAllStatus()).Where(s => s.Order > 0 && s.Order < 12).ToList();
            phases = (await _phaseRepository.getAllAsync()).Where(p => p.Order > 0).ToList();
            FillListOfPage(Constants.getPageSize(laws_.Count));
            isLoad = false;
        }
        catch (Exception ex)
        {
            isLoad = false;
            _logger.LogError(ex.Message, $"Error On {OnInitializedAsync}", nameof(FollowLaws), null);
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Methode Fill List Number For Table Pagination === 
    */
    void FillListOfPage(int count)
    {
        for (int i = 1; i <= count; i++)
            pages.Add(i);
    }

    /*
     * === Set Law Informations ===
    */
    protected async Task HandleSetLawInfo(Guid Id)
    {
        LawInformationId = Id;
        await jsRuntime.InvokeVoidAsync("ShowCanvas", "offcanvasTopEditLawInfo");
    }

    /*
     * === Fill List Of Law By User Role === 
    */
    protected async Task LoadLaws()
    {
        try
        {
            if (stateContainerService.user!.Roles.Count > 0)
            {
                laws.Clear();
                if (stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_COMMISSION)))
                    laws = await _lawRepository.GetLawsForCommission(stateContainerService.user.TeamsDtos.Select(t => t.Id).ToList(), stateContainerService.user.Id);
                else if (stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_SEANCE)))
                    laws = await _lawRepository.GetLawsForSession(null!, stateContainerService.user.Id);
                else if (stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_LEGISLATION)))
                    laws = await _lawRepository.GetLawsForlegislation();
                else
                    laws = await _lawRepository.GetLawsForDirector(stateContainerService.user.TeamsDtos.Select(t => t.Id).ToList(), stateContainerService.user.Id);
                laws_ = laws.ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {LoadLaws}", nameof(FollowLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * ===  Get Nodes Of Law ===
     */
    protected async Task LoadLawNodes(Guid LawId)
    {
        EditlawContentId = LawId;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModaleEditLaw");
    }

    /*
     * === Filter By List Laws ===
    */
    private void Filter(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            laws = laws_;
        if (filterBy == "Label")
            laws = laws_.Where(l => l.Label.Contains(e.Value!.ToString()!)).ToList();
        if (filterBy == "PhaseLawId")
        {
            if (e.Value.ToString() != string.Empty)
            {
                laws = laws_.Where(l => l.PhaseLawId == Guid.Parse(e.Value!.ToString()!)).ToList();
            }
        }
        if (filterBy == "CommissionName")
        {
            if (e.Value.ToString() != string.Empty)
            {
                laws = laws_.Where(l => l.CommissionName.Equals(e.Value.ToString())).ToList();
            }
        }
        if (filterBy == "TeamName")
        {
            if (e.Value.ToString() != string.Empty)
            {
                laws = laws_.Where(l => l.TeamName == e.Value.ToString()).ToList();
            }
        }
        if (filterBy == "Statu")
        {
            if (e.Value.ToString() != string.Empty)
            {
                laws = laws_.Where(l => l.StatuName == e.Value.ToString()).ToList();
            }
        }

        if (filterBy == "Number")
        {
            if (e.Value.ToString() != string.Empty)
            {
                laws = laws_.Where(l => l.Number.Contains(e.Value!.ToString()!)).ToList();
            }
        }
        if (filterBy == "Year")
            laws = laws_.Where(l => l.Year.ToString().Contains(e.Value!.ToString()!)).ToList();
    }


    /*
     * === Submit Law To Commission ===
    */
    private async Task SubmitLawToCommission(Guid LawId, Guid phaseId)
    {
        try
        {
            startSubmitedText = true;
            Guid PhasLawId = Guid.Empty;
            if (phaseId == Guid.Parse(_phaseOptions.Value.PHASE_AFFECTATION_BUREUA_1))
                PhasLawId = Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1);
            else
                PhasLawId = Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_2);
            var response = await _nodeRepository.SetPhaseNodes(LawId, PhasLawId, stateContainerService.user!.Id, (int)LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_COMMISSION);
            await Refresh(response.Flag);
            if ((response.Flag))
            {
                await _traceService.insertTrace(new Trace { Operation = "Submit Law To Commission", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }
            startSubmitedText = false;
        }
        catch (Exception ex)
        {
            startSubmitedText = false;
            _logger.LogError(ex.Message, $"Error on {nameof(SubmitLawToCommission)}", nameof(FollowLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Region Refresh Data ===
    */
    private async Task Refresh(bool IsSuccess)
    {
        if (IsSuccess)
        {
            toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            await LoadLaws();
        }
        else
        {
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Browse Law ===
    */
    async Task BrowseLaw(Guid LawId_)
    {
        BrowsLawId = LawId_;
        await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasBrowsLaw");
    }

    /*
     * === Pagination ===
     */
    private void paginate(int page)
    {

        if (page > curentIndex)
        {
            laws = laws_.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
        else
        {
            laws = laws_;
            int take = page - 1 == 0 ? 1 : page - 1;
            int skip = page - 1;
            laws = laws.Skip(skip).Take(pageSize * take).ToList();
        }
        curentIndex = page;
    }
    private void lastPage()
    {
        int lastPgae = pages.Last();
        laws = laws_.Skip((lastPgae - 1) * pageSize).Take(pageSize).ToList();
    }
    private void firstPage()
    {
        laws = laws_;
        laws = laws.Skip(0).Take(pageSize * 1).ToList();
    }


    /*
     * === Display Law Documents ===
    */
    private async Task ShowLawDocuments(Guid LawId_)
    {
        LawId = LawId_;
        await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasDocuments");
    }

    /*
     * === Clone Node Of Law
    */
    private async Task cloneNodeLawToDestinationPhase(Guid lawId)
    {
        try
        {
            isClone = true;
            var law = await _lawRepository.GetByIdAsync(lawId);
            LawId = lawId;
            Guid PhaseId = Guid.Empty;
            if (stateContainerService.user!.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_COMMISSION)))
            {
                var phaseComOne = law.PhaseLawIds.FirstOrDefault(p => p.PhaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1));
                if (phaseComOne!.Statu!.Equals(PhaseStatu.OPENED.ToString()))
                {
                    PhaseId = Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1);
                }
                else
                {
                    PhaseId = Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_2);
                }
                if (law?.Category == "PLF")
                {
                    SectionLawId = LawId;
                    nodeSection = await _nodeRepository.GetLawSections(LawId, PhaseId, false);
                    await jsRuntime.InvokeVoidAsync(Constants.ShowModal, "ModalClonePlf");
                }
                else
                {
                    var curentPhase = await _phaseRepository.getPhaseById(PhaseId);
                    var destinationPhase = await _phaseRepository.getPhaseByOrder(curentPhase.Order + 1);
                    var response = await _amendmentRepository.CheckAmendmentsHasNewContent(LawId, PhaseId);
                    if (response.Flag)
                    {
                        toastService.ShowError(response.Massage, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    }
                    else
                    {

                        var statu = await _statuRepository.getStatusById(law.StatuId ?? Guid.Empty);
                        if (!ValidateLawStatu(PhaseId, statu.Order ?? 0))
                        {
                            toastService.ShowError(" المرجو تغيير وضعية النص القانوني قبل نسخ العقد.", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                        }
                        else
                        {
                            var serverResponse = await _nodeRepository.CloneNodes(LawId, PhaseId, destinationPhase.Id, PhaseStatu.OPENED.ToString());
                            await Refresh(response.Flag);
                        }
                    }
                }
            }
            else
            {
                var phaseSeanceOne = law.PhaseLawIds.FirstOrDefault(p => p.PhaseId == Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1));
                if (phaseSeanceOne!.Statu!.Equals(PhaseStatu.OPENED.ToString()))
                {
                    PhaseId = Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1);
                }
                else
                {
                    PhaseId = Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_2);
                }
                var curentPhase = await _phaseRepository.getPhaseById(PhaseId);
                var destinationPhase = await _phaseRepository.getPhaseByOrder(curentPhase.Order + 1);
                var response = await _amendmentRepository.CheckAmendmentsHasNewContent(LawId, PhaseId);
                if (response.Flag)
                {
                    toastService.ShowError(response.Massage, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
                else
                {

                    var statu = await _statuRepository.getStatusById(law.StatuId ?? Guid.Empty);
                    if (!ValidateLawStatu(PhaseId, statu.Order ?? 0))
                    {
                        toastService.ShowError(" المرجو تغيير وضعية النص القانوني قبل نسخ العقد.", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    }
                    else
                    {
                        var serverResponse = await _nodeRepository.CloneNodes(LawId, PhaseId, destinationPhase.Id, PhaseStatu.CLOSED.ToString());
                        await Refresh(!response.Flag);
                        await _traceService.insertTrace(new Trace { Operation = "Clone Nodes", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                    }
                }
            }
            isClone = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {cloneNodeLawToDestinationPhase}", nameof(FollowLaws), null);
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            isClone = false;
        }
    }

    public bool ShowAction(List<string> phaseIds)
    {
        if (stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.MEMBER_SEANCE))
        {
            if (phaseIds.Contains(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1.ToLower()) || phaseIds.Contains(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_2.ToLower()))
                return true;
            else
                return false;
        }
        if (stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.MEMBER_COMMISSION))
        {
            if (phaseIds.Contains(_phaseOptions.Value.PHASE_COMMISSION_1.ToLower()) || phaseIds.Contains(_phaseOptions.Value.PHASE_COMMISSION_2.ToLower()))
                return true;
            else
                return false;
        }
        if (stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.DIRECTEUR_LEGISLATION))
            return true;
        if (stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.MEMBER_LEGISLATION))
            return true;
        return false;
    }

    /*
     * === Validate Law Statu ===
    */
    bool ValidateLawStatu(Guid phaseId, int order)
    {
        //var Order = await 
        if (phaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1))
            return order == 4;
        if (phaseId == Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_2))
            return order == 10;
        if (phaseId == Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1))
            return order == 5;
        if (phaseId == Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_2))
            return order == 11;
        return false;
    }

    private async Task setLawInfo(bool response)
    {
        if (response)
        {
            await _traceService.insertTrace(new Trace { Operation = "Set Law Information", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            await Refresh(response);
        }
    }


    public async Task cloneLawSection()
    {
        try
        {
            startCloneSection = true;
            var response = await _amendmentRepository.CheckAmendmentsSectionHasNewContent(sectionId);
            if (response.Flag)
            {
                toastService.ShowError(response.Massage, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                var law = await _lawRepository.GetByIdAsync(LawId);
                var statu = await _statuRepository.getStatusById(law.StatuId ?? Guid.Empty);
                if (!ValidateLawStatu(Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1), statu.Order ?? 0))
                {
                    toastService.ShowError(" المرجو تغيير وضعية النص القانوني قبل نسخ العقد.", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                }
                else
                {
                    var resp = await _nodeRepository.cloneSectionWithChildrens(sectionId, Guid.Parse(_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1));
                    await _traceService.insertTrace(new Trace { Operation = "Clone Section", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                }
            }
            startCloneSection = false;
            LawId = Guid.Empty;
            await jsRuntime.InvokeVoidAsync(Constants.HideModal, "ModalClonePlf");
        }
        catch (Exception ex)
        {
            startCloneSection = false;
            _logger.LogError(ex.Message, $"Error On {cloneLawSection}", nameof(FollowLaws), null);
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    public void ChoseSection(ChangeEventArgs e)
    {
        sectionId = Guid.Parse(e.Value!.ToString()!);
    }


    public async Task RedirectToOrderAmendmernts(Guid lawId)
    {
        string PhaseId = string.Empty;
        var law = await _lawRepository.GetByIdAsync(lawId);
        if (stateContainerService.user.Roles!.Any(r => r.Name == _roleOptions.Value.MEMBER_SEANCE))
        {
            PhaseId = law.PhaseLawIds.FirstOrDefault(p => (p.PhaseId?.ToString().ToUpper() == _phaseOptions.Value.PHASE_SEANCE_PLENIERE_1.ToString()
            || p.PhaseId?.ToString().ToUpper() == _phaseOptions.Value.PHASE_SEANCE_PLENIERE_2.ToString()) && p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId.ToString()!;
        }
        else
        {
            PhaseId = law.PhaseLawIds.FirstOrDefault(p => (p.PhaseId?.ToString().ToUpper() == _phaseOptions.Value.PHASE_COMMISSION_1.ToString()
            || p.PhaseId?.ToString().ToUpper() == _phaseOptions.Value.PHASE_COMMISSION_2.ToString()) && p.Statu == PhaseStatu.OPENED.ToString())?.PhaseId.ToString()!;
        }
        _navigationManager.NavigateTo($"/order-amendments/{lawId}/{PhaseId}");
    }
}


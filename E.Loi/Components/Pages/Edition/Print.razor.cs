namespace E.Loi.Components.Pages.Edition;

public partial class Print
{
    List<TeamVm> commissions = new();
    List<Phase> phases = new();
    List<Phase> phases_ = new();
    private Guid ComId = Guid.Empty, PhaseId = Guid.Empty, LawId = Guid.Empty, sectionId = Guid.Empty;
    private List<LawVm> laws = new();
    private List<LawVm> laws_ = new();
    private List<string> Errors = new();
    private List<Statut> status = new();
    public FlatNode[] nodeSection = new FlatNode[0];
    private bool startProcesse = false, IsPdf = false, IsWord = false, startDownloading = false, startSearch = false, IsWordVotingFile = false, IsPdfVotingFile;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            commissions = await _teamRepository.GetCommissionsAsync();
            List<int> orders = new() { 0, 1, 5 };
            phases_ = (await _phaseRepository.getAllAsync()).Where(p => p.Order > 0).ToList();
            status = (await _statuRepository.getAllStatus()).Where(s => s.Order > 0 && s.Order < 12).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {OnInitializedAsync}", nameof(Print));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    private void setComId(ChangeEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Value!.ToString()))
            ComId = Guid.Parse(e.Value.ToString()!);
    }
    private async Task setPhaseId(ChangeEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Value!.ToString()))
        {
            PhaseId = Guid.Parse(e.Value.ToString()!);
            nodeSection = await _nodeRepository.GetLawSections(LawId, PhaseId, true);
        }
    }

    private async Task setLawId(Guid Id)
    {
        phases.Clear();
        LawId = Id;
        foreach (var law_ in laws)
            law_.HasAction = false;
        var law = laws.First(l => l.Id == Id);
        law.HasAction = true;
        var selecteedLaw = await _lawRepository.GetByIdAsync(Id);
        //  nodeSection = await _nodeRepository.GetLawSections(LawId, PhaseId);
        var phase = selecteedLaw.PhaseLawIds.Select(p => new Phase { Id = p.Phases.Id, Title = p.Phases.Title, Order = p.Phases.Order }).ToList();
        phases = phase.Where(p => p.Order > 1 && p.Order != 5).ToList();
    }

    private async Task getLaws()
    {
        try
        {
            if (ComId != Guid.Empty)
            {
                startProcesse = true;
                laws = laws_ = await _lawRepository.GetLawsToPrint(ComId);
                startProcesse = false;
                startSearch = !(laws.Count > 0);
            }
            else { }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {OnInitializedAsync}", nameof(Print));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
    private void FilterLaws(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            laws = laws_;
        if (filterBy == "Label")
        {
            laws = laws_.Where(l => l.Label.Contains(e.Value!.ToString()!)).ToList();
        }

        if (filterBy == "Statu")
        {
            if (e.Value.ToString() != string.Empty)
            {
                laws = laws_.Where(l => l.StatuName == e.Value.ToString()).ToList();
            }
        }
        startSearch = !(laws.Count > 0);
    }

    private void setIsPdf(ChangeEventArgs e) => IsWord = !(bool)e.Value!;
    private void setIsWord(ChangeEventArgs e) => IsPdf = !(bool)e.Value!;
    private void setIsWordVotingFile(ChangeEventArgs e) => IsPdfVotingFile = !(bool)e.Value!;
    private void setIsPdfVotingFile(ChangeEventArgs e) => IsWordVotingFile = !(bool)e.Value!;
    private async Task DownloadLaw()
    {
        try
        {
            if (LawId != Guid.Empty && PhaseId != Guid.Empty)
            {
                string outType = IsPdf ? "pdf" : "docx";
                startDownloading = true;
                var law = await _lawRepository.GetByIdAsync(LawId);
                var bytes = await _editionRepository.PrintTextLaw(LawId, PhaseId, outType);
                await _printService.DownloadTextLaw(bytes, law.Label + "." + outType);
                startDownloading = false;
                await _traceService.insertTrace(new Trace { Operation = $"Print Law {outType}", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }
            else
            {
                Errors.Clear();
                if (LawId == Guid.Empty)
                    Errors.Add(".المرجو إختيار النص المراد طباعته");
                if (PhaseId == Guid.Empty)
                    Errors.Add("المرجو إختيار المرحلة.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {OnInitializedAsync}", nameof(Print));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            startDownloading = false;
        }

    }

    private async Task PrintVotingFile()
    {
        try
        {
            string outType = IsPdfVotingFile ? "pdf" : "docx";
            startDownloading = true;
            var bytes = await _editionRepository.GenerateVotingFile(LawId, sectionId, outType);
            await _printService.DownloadTextLaw(bytes, "ملف التصويت" + "." + outType);
            startDownloading = false;
            await _traceService.insertTrace(new Trace { Operation = $"Print Law {outType}", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {OnInitializedAsync}", nameof(Print));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            startDownloading = false;
        }

    }



    private async Task printPresident()
    {
        try
        {
            startDownloading = true;
            string outType = IsPdfVotingFile ? "pdf" : "docx";
            startDownloading = true;
            var bytes = await _editionRepository.PrintVotingFileForPresident(Guid.Parse("8346C3F8-2403-43E1-99A9-69176B71F524"), Guid.Parse("487EB5EA-8A1E-4D2A-B276-08DB13F5E309"), "pdf");
            await _printService.DownloadTextLaw(bytes, "ملف التصويت" + "." + "pdf");
            startDownloading = false;
            await _traceService.insertTrace(new Trace { Operation = $"Print Law {outType}", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            startDownloading = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {OnInitializedAsync}", nameof(Print));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            startDownloading = false;
        }

    }
    public void ChoseSection(ChangeEventArgs e)
    {
        sectionId = Guid.Parse(e.Value!.ToString()!);
    }
}

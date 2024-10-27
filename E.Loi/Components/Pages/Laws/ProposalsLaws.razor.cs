using Microsoft.AspNetCore.Components.Forms;

namespace E.Loi.Components.Pages.Laws;

public partial class ProposalsLaws
{

    /*
     * === Global Variabls ===
    */
    [SupplyParameterFromForm]
    private EditLawVm model { get; set; } = new() { Year = DateTime.Now.Year };
    List<LawVm> laws = new();
    List<LawVm> laws_ = new();
    List<DocumentVm> documents = new();
    public List<NodeVm>? nodesToBrows { get; set; }
    private bool IsLoad = false, isDownload = false, startSubmitedText = false;
    private long maxFileSize = 1024 * 1024000;
    private Guid LawId, BrowsLawId;
    private string pathLawtext = string.Empty;
    DocumentTexteLoiVM? TextLawDoc;
    List<int> pages = new();
    int curentIndex = 0, pageSize = 10;
    List<Phase> phases = new();
    /*
     * === OnInitializedAsync ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadLaws();
            pages = Constants.FillListOfPage(Constants.getPageSize(laws_.Count));
            phases = await _phaseRepository.getAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(OnInitializedAsync)}", nameof(ProposalsLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    /*
     * === Method Bind Laws ===
    */
    async Task LoadLaws()
    {
        IsLoad = true;
        if (stateContainerService.user.Roles.Any(role => role.Name == _roleOptions.Value.MEMBER_GROUPE))
        {
            laws = laws_ = await _lawRepository.GetPreparationLawsAsync(stateContainerService.user!.TeamsDtos.Select(t => t.Id).ToList());
        }
        else
        {
            laws = laws_ = await _lawRepository.GetAllPreparationLawsAsync(Guid.Parse(_phaseOptions.Value.PREPARATION_PHASE), stateContainerService.user.TeamId);
        }
        laws = laws.Skip(0).Take(pageSize).ToList();
        IsLoad = false;
    }



    /*
     * === Chose Law Text File ===
     */
    private async void ChoseLawTextFile(InputFileChangeEventArgs e)
    {
        pathLawtext = Path.Combine(_environment.WebRootPath, "TextLaws", e.File.Name);
        var memory = new MemoryStream();
        FileStream fs = new(pathLawtext, FileMode.Create);
        await e.File.OpenReadStream(maxFileSize).CopyToAsync(fs);
        fs.Close();
        var stream = new FileStream(pathLawtext, FileMode.Open);
        stream.CopyTo(memory);
        TextLawDoc = new();
        TextLawDoc.texteId = 134;//TDOD
        TextLawDoc.FileName = e.File.Name;
        TextLawDoc.Bytes = memory.ToArray();
        stream.Close();
        memory.Close();
        File.Delete(pathLawtext);
    }

    /*
     * === Create Nodes From Law Text File ===
    */
    private async Task CreateNodesFromlawTextFile()
    {
        try
        {
            isDownload = true;
            var texts = await _lawRepository.GetTextLawFromFile(TextLawDoc!);
            var response = await _nodeRepository.CreateLawNodes(texts, LawId, stateContainerService.user!.Id);
            await jsRuntime.InvokeVoidAsync("HideModal", "MoadlUploadTextLaw");
            if (response.Flag)
                toastService.ShowSuccess(Constants.MessageLoadLawContent, settings => { settings.ShowCloseButton = false; settings.Position = Blazored.Toast.Configuration.ToastPosition.BottomCenter; });
            else
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = Blazored.Toast.Configuration.ToastPosition.BottomCenter; });
            isDownload = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(CreateNodesFromlawTextFile)}", nameof(ProposalsLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Show Modal Upload Law Text ===
    */
    public async Task ShowModalUploadLawText(Guid Idlaw)
    {
        LawId = Idlaw;
        await jsRuntime.InvokeVoidAsync("ShowModal", "MoadlUploadTextLaw");
    }

    /*
     * === Set Law And Nodes Phase ===
    */
    private async Task SetPhaseNodes(Guid LawId, Guid phaseId)
    {
        try
        {
            startSubmitedText = true;
            Guid PhasLawId = Guid.Empty;
            if (phaseId == Guid.Parse(_phaseOptions.Value.PREPARATION_PHASE))
            {
                PhasLawId = Guid.Parse(_phaseOptions.Value.PHASE_AFFECTATION_BUREUA_1);
            }
            else
            {
                PhasLawId = Guid.Parse(_phaseOptions.Value.PHASE_AFFECTATION_BUREUA_2);
            }
            var response = await _nodeRepository.SetPhaseNodes(LawId, PhasLawId, stateContainerService.user!.Id, (int)LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_OFFICE_COUNCIL);
            if (response.Flag)
            {
                toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                await LoadLaws();
                await _traceService.insertTrace(new Trace { Operation = "Submit Law To Bureau", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            startSubmitedText = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(SetPhaseNodes)}", nameof(ProposalsLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Filter By Law Label ===
    */
    private void FilterBylabel(ChangeEventArgs e)
    {
        if (e.Value!.ToString() == string.Empty)
            laws = laws_;
        else
            laws = laws_.Where(l => l.Label.Contains(e.Value!.ToString()!)).ToList();
    }




    /*
     * === Downloade Law File ===
    */
    private async Task ShowLawDocuments(Guid lawId)
    {
        LawId = lawId;
        await jsRuntime.InvokeVoidAsync("ShowCanvas", "canvasDocuments");
    }

    /*
     * === Pagination ===
    */
    private void paginate(int page)
    {

        if (page > curentIndex)
            laws = laws_.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        else
            laws = laws_.Skip(page - 1).Take(pageSize * (page - 1 == 0 ? 1 : page - 1)).ToList();
        curentIndex = page;
    }
    private void lastPage()
    {
        laws = laws_.Skip((pages.Last() - 1) * pageSize).Take(pageSize).ToList();
    }
    private void firstPage()
    {
        laws = laws_.Skip(0).Take(pageSize * 1).ToList();
    }
    /*
     * === Delete Law ===
    */
    private async Task deleteLaw(Guid LawId)
    {
        try
        {
            var response = await _lawRepository.DeleteLawAsync(LawId, stateContainerService.user!.Id);
            if (response.Flag)
            {
                toastService.ShowSuccess(Constants.SuccessDeleteOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                await LoadLaws();
                await _traceService.insertTrace(new Trace { Operation = "Delete Law", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(SetPhaseNodes)}", nameof(deleteLaw));
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
     * === Handle Close Canvase ===
     */
    private async Task onAdd(bool isSuccess)
    {
        if (isSuccess)
            await LoadLaws();
    }

}


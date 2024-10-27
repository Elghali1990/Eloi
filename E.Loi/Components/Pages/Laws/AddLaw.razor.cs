using Microsoft.AspNetCore.Components.Forms;

namespace E.Loi.Components.Pages.Laws;

public partial class AddLaw
{
    [SupplyParameterFromForm]
    private EditLawVm model { get; set; } = new() { Year = DateTime.Now.Year };
    private bool IsSelectPdf = true, IsSelectWord = true, isProcessing = false;
    List<Phase> phases = new();
    private long maxFileSize = 1024 * 1024000;
    private string pathLawtext = string.Empty, pathPdf = string.Empty, pathWord = string.Empty;
    [Parameter]
    public EventCallback<bool> OnAdd { get; set; }
    [CascadingParameter]
    public Error? Error { get; set; }
    protected override async Task OnInitializedAsync()
    {
        phases = await _phaseRepository.getAllAsync();
    }

    /*
    * === Select law File ===
   */
    private async Task<string> SelectFile(InputFileChangeEventArgs e)
    {
        try
        {
            string path = Path.Combine(_environment.WebRootPath, "TextLaws", e.File.Name);
            await using FileStream fs = new(path, FileMode.Create);
            await e.File.OpenReadStream(maxFileSize).CopyToAsync(fs);
            fs.Close();
            return path;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(SelectFile)}", nameof(ProposalsLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            return null!;
        }
    }

    /*
  * === Select Pdf File ===
  */
    private async Task SelectPDFLaw(InputFileChangeEventArgs e)
    {
        pathPdf = await SelectFile(e);
    }


    /*
     * === Select Pdf File ===
    */
    private async Task SelectWORDLaw(InputFileChangeEventArgs e)
    {
        pathWord = await SelectFile(e);
    }

    protected async Task HandleEventAddLaw()
    {
        try
        {
            isProcessing = true;
            var check = await _lawRepository.CheckLawExiste(model.Number!, model.Year);
            if (check.Flag)
            {
                toastService.ShowError(Constants.TextAlreadyExists, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                var pdf = pathPdf;
                var word = pathWord;
                model.CreatedBy = stateContainerService.user!.Id;
                model.TeamId = stateContainerService.user!.TeamId;
                IsSelectPdf = pathPdf.Equals(string.Empty) ? false : true;
                IsSelectWord = pathPdf.Equals(string.Empty) ? false : true;
                if (stateContainerService.user.Roles.Any(r => r.Name == _roleOptions.Value.MEMBER_GROUPE))
                {
                    model.PhaseId = phases.FirstOrDefault(p => p.Order == 0)!.Id;
                    model.Category = "مقترح قانون";
                }
                if (IsSelectPdf && IsSelectWord)
                {
                    var law = await _lawRepository.AddLawAsync(model);
                    if (law is not null)
                    {
                        CreateNodeVm nodeVm = new();
                        nodeVm.Label = law.Label;
                        nodeVm.TypeId = Guid.Parse("665AF4BD-DECA-47CC-7DD1-08DA3CE94CE8");
                        nodeVm.Content = string.Empty;
                        nodeVm.OriginalContent = string.Empty;
                        nodeVm.PhaseLawId = law.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())!.PhaseId;
                        nodeVm.LawId = law.Id;
                        nodeVm.ParentNodeId = null;
                        nodeVm.CreatedBy = stateContainerService.user!.Id;
                        var result = await _documentRepository.InsertDocument(new List<DocumentVm>(){
                        new DocumentVm{ LawId =law.Id,Path=pathPdf,Type="PDF",DocumentName=Path.GetFileName(pathPdf).Split(".")[0]},
                        new DocumentVm { LawId = law.Id, Path = pathWord, Type = "WORD",DocumentName = Path.GetFileName(pathWord).Split(".")[0]}
                        });
                        var node = await _nodeRepository.CreateNode(nodeVm);
                        await jsRuntime.InvokeVoidAsync(Constants.HideModal, "AddNewlawModal");
                        toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                        await OnAdd.InvokeAsync(true);
                        await _traceService.insertTrace(new Trace { Operation = "Add New Law ", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                        model = new();
                    }
                }
            }
            isProcessing = false;


        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(HandleEventAddLaw)}", nameof(ProposalsLaws));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


}

using Microsoft.AspNetCore.Components.Forms;

namespace E.Loi.Components.Pages.Utils;

public partial class LawDocuments
{
    /*
     * ===Global variables for component ===
    */
    [Parameter]
    public Guid LawId { get; set; }
    List<DocumentVm> documents = new();
    private string PdfBase64 = string.Empty;
    private string DocType = string.Empty, DocName = string.Empty, filePath = string.Empty;
    private bool showBoxUploadFile = false, IsFileExist = true;
    /*
     * === Component Set Parametre ===
    */
    protected override async Task OnParametersSetAsync()
    {
        try
        {
            if (LawId != Guid.Empty)
            {

                documents = await _documentRepository.GetLawDocumentsAsync(LawId);
                if (stateContainerService.user!.Roles!.Any(role => role.Name == _roleOptions.Value.MEMBER_GROUPE))
                {
                    var law = await _lawRepository.GetByIdAsync(LawId);
                    showBoxUploadFile = law.Statu?.Order == 0 ? true : false;
                }
                else
                {
                    showBoxUploadFile = true;
                }
                PdfBase64 = string.Empty;
                IsFileExist = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(OnParametersSetAsync)}", nameof(LawDocuments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
      * === Download Document ===
    */
    private async Task DownloadeFile(Guid docId) => await GetDocumentFromStream(docId);

    /*
     * === Get Document From Stream Of Bytes ===
    */

    private async Task GetDocumentFromStream(Guid DocId)
    {
        try
        {
            IsFileExist = true;
            var bytes = await ConvertDocumentToArrayOfBytes(DocId);
            if (bytes != null)
            {
                var fileStream = new MemoryStream(bytes);
                string extention = DocType == "PDF" ? "pdf" : "docx";
                using var streamRef = new DotNetStreamReference(stream: fileStream);
                using var rfStream = new DotNetStreamReference(stream: fileStream);
                await jsRuntime.InvokeVoidAsync("downloadFileFromStream", DocName, rfStream);
            }
            else
            {
                IsFileExist = false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(GetDocumentFromStream)}", nameof(LawDocuments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Parse Document To Array Of Bytes ===
    */
    private async Task<byte[]> ConvertDocumentToArrayOfBytes(Guid DocId)
    {
        var response = await _documentRepository.GetDocumentByIdAsync(DocId);
        if (response is not null)
        {
            if (File.Exists(response.Path))
            {
                DocType = response.Type == "PDF" ? "pdf" : "docx";
                DocName = response.DocumentName + "." + DocType;
                var ms = new MemoryStream();
                var stream = new FileStream(response.Path, FileMode.Open);
                stream.CopyTo(ms);
                Byte[] bytes = ms.ToArray();
                stream.Close();
                ms.Close();
                return bytes;
            }
        }
        return null!;
    }

    /*
     * === Event Close Canvas ===
    */
    private async Task HideCanvas()
    {
        PdfBase64 = string.Empty;
        IsFileExist = true;
        await jsRuntime.InvokeVoidAsync("HideCanvas", "canvasDocuments");
    }

    /*
     * === Convert Array Of Bytes To Base64 ===
    */
    private string ConvertBytesTo64Base(byte[] bytes)
    {
        var base64String = Convert.ToBase64String(bytes);
        return $"data:application/pdf;base64,{base64String}";
    }

    /*
     * === Brows Document  Into Ifram ===
    */
    private async Task BrowsDocument(Guid DocId)
    {
        IsFileExist = true;
        var bytes = await ConvertDocumentToArrayOfBytes(DocId);
        if (bytes != null)
        {
            PdfBase64 = ConvertBytesTo64Base(bytes);
        }
        else
        {
            IsFileExist = false;
        }
    }

    /*
     * === Chose File From Input File ===
    */
    private async Task SelectFile(InputFileChangeEventArgs e)
    {
        try
        {
            string path = Path.Combine(Environment.WebRootPath, "TextLaws", e.File.Name);
            await using FileStream fs = new(path, FileMode.Create);
            await e.File.OpenReadStream(1024 * 1024000).CopyToAsync(fs);
            fs.Close();
            filePath = path;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(SelectFile)}", nameof(LawDocuments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Add Law Document ===
    */
    private async Task InsertDocument()
    {
        try
        {
            DocumentVm document = new()
            {
                LawId = LawId,
                Path = filePath,
                DocumentName = Path.GetFileName(filePath).Split(".")[0],
                Type = Path.GetExtension(filePath) == ".pdf" ? "PDF" : "WORD",
            };
            var response = await _documentRepository.InsertDocument(new List<DocumentVm> { document });
            if (response.Flag)
            {
                toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = Blazored.Toast.Configuration.ToastPosition.BottomCenter; });
                PdfBase64 = string.Empty;
                documents = await _documentRepository.GetLawDocumentsAsync(LawId);
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = Blazored.Toast.Configuration.ToastPosition.BottomCenter; });
            }
            StateHasChanged();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(SelectFile)}", nameof(LawDocuments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Delete Document ===
    */
    public async Task DeleteDocument(Guid documentId)
    {
        try
        {
            var document = await _documentRepository.GetDocumentByIdAsync(documentId);
            if (document is not null)
            {
                if (File.Exists(document.Path))
                {
                    File.Delete(document.Path);
                }
                var response = await _documentRepository.DeleteDocumentAsync(documentId);
                if (response.Flag)
                {
                    documents = await _documentRepository.GetLawDocumentsAsync(LawId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(DeleteDocument)}", nameof(LawDocuments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
}

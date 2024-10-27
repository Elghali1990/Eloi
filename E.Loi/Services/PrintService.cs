namespace E.Loi.Services;

public class PrintService(IEditionRepository _editionRepository, IJSRuntime _jsRuntime)
{
    public async Task PrintNodeContent(Guid NodeId)
    {
        var bytes = await _editionRepository!.PrintNode(NodeId);
        if (bytes is not null)
        {
            var fileStream = new MemoryStream(bytes);
            var fileName = "node.pdf";
            await setStream(fileStream, fileName);
        }
    }

    public async Task DownloadAmendments(byte[] bytes, string Label, int Year, string Number, string OutType)
    {
        var fileStream = new MemoryStream(bytes);
        var fileName = "تعديلات " + Label + "لسنة " + Year + " رقم " + Number + "." + OutType;
        await setStream(fileStream, fileName);
    }


    public async Task DownloadAmendments(byte[] bytes, string teamName, string OutType)
    {
        var fileStream = new MemoryStream(bytes);
        var fileName = "تعديلات " + teamName + "." + OutType;
        await setStream(fileStream, fileName);
    }
    public async Task DownloadTextLaw(byte[] bytes, string fileName)
    {
        var fileStream = new MemoryStream(bytes);
        await setStream(fileStream, fileName);
    }

    public async Task setStream(MemoryStream memoryStream, string fileName)
    {
        using var rfStream = new DotNetStreamReference(stream: memoryStream);
        await _jsRuntime.InvokeVoidAsync("downloadFileFromStream", fileName, rfStream);
    }
}

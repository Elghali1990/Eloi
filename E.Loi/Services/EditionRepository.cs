namespace E.Loi.Services;

public class EditionRepository(IHttpClientFactory clientFactory) : IEditionRepository
{
    HttpClient httpClient = clientFactory.CreateClient("edition");

    public async Task<byte[]> GenerateVotingFile(Guid LawId, Guid SectionId, string outType)
    {
        try
        {
            var response = await httpClient.GetAsync($"VotingFileSession/generateVotingFile/{LawId}/{SectionId}?outType={outType}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<byte[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<byte[]> PrintEditorContent(EditorContent editorContent)
    {

        try
        {
            var response = await httpClient.PostAsJsonAsync("PrintNodes/printEditorContent", editorContent);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<byte[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<byte[]> PrintNode(Guid NodeId)
    {
        try
        {
            var response = await httpClient.GetAsync($"PrintNodes/printNode/{NodeId}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<byte[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<byte[]> PrintTeamAmendments(SetAmendData data)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("PrintAmendments", data);
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<byte[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
    public async Task<byte[]> PrintTextLaw(Guid LawId, Guid PhaseLawId, string outType)
    {
        try
        {
            var response = await httpClient.GetAsync($"TextLaw/generateTextLaw?LawId={LawId}&PhaseLawId={PhaseLawId}&outType={outType}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<byte[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<byte[]> printVoteAmendmentsResult(Guid LawId, Guid PhaseLawId, string outType)
    {
        try
        {
            var response = await httpClient.GetAsync($"PrintAmendments/printVoteAmendmentsResult?LawId={LawId}&PhaseLawId={PhaseLawId}&outType={outType}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<byte[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<byte[]> printVotingFileCommission(Guid LawId, Guid PhaseLawId, string outType, bool includeAmendmentRatraper)
    {
        try
        {
            var response = await httpClient.GetAsync($"PrintAmendments/printVotingFileCommission?LawId={LawId}&PhaseLawId={PhaseLawId}&outType={outType}&includeAmendmentRatraper={includeAmendmentRatraper}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<byte[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }

    public async Task<byte[]> PrintVotingFileForPresident(Guid LawId, Guid SectionId, string outType)
    {
        try
        {
            var response = await httpClient.GetAsync($"VotingFileSession/printVotingFileForPresident/{LawId}/{SectionId}?outType={outType}");
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<byte[]>())!;
            return null!;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException(ex.Message);
        }
    }
}


namespace E.Loi.Echange.Passerelle.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PasserelleController(IHttpClientFactory _httpClientFactory, IHttpClientFactory _httpClientFactoryMF, ILogger<PasserelleController> _logger) : ControllerBase
{
    HttpClient _httpClient = _httpClientFactory.CreateClient("BaseApiUrl");
    HttpClient _httpClientMF = _httpClientFactoryMF.CreateClient("BaseUrlFinance");

    [HttpPost]
    [Route("getAllTeamsAsync")]
    public async Task<IActionResult> GetAllTeamsAsync([FromBody] List<TeamDto> teamDtos)
    {
        try
        {
            var responseMF = await _httpClientMF.PostAsJsonAsync("ePlfProcessReceiveApi/createTeam", teamDtos);
            if (responseMF.IsSuccessStatusCode)
            {
                return Ok(new ServerResponse(true, "Teams created successfully."));
            }
            return BadRequest(new ServerResponse(false, "Teams not created."));
            // return Ok(new ServerResponse(true, "Teams created successfully."));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, $"Error when calling {nameof(GetAllTeamsAsync)}", nameof(PasserelleController));
            return BadRequest($"Error when colling {nameof(GetAllTeamsAsync)}");
        }
    }

    [HttpPost]
    [Route("createLawAsync")]
    public async Task<IActionResult> createLawAsync([FromBody] LawDto lawDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Laws/createLawAsync", lawDto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ServerResponse>();
                return Ok(response.StatusCode);
            }
            return Ok(response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(createLawAsync)}", nameof(PasserelleController));
            return BadRequest("Error when create law");
        }
    }

    [HttpPost]
    [Route("createNodesAsync/{LawId:guid}")]
    public async Task<IActionResult> createNodesAsync([FromBody] NodeDto[] NodeDtos, Guid LawId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"Nodes/createNodesAsync/{LawId}", NodeDtos);
            if (response.IsSuccessStatusCode)
            {
                // var result = await response.Content.ReadFromJsonAsync<ServerResponse>();
                return Ok(response.StatusCode);
            }
            return Ok(response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(createNodesAsync)}", nameof(PasserelleController));
            return BadRequest("Error when create law and nodes");
        }
    }




    [HttpPost]
    [Route("insertGovernmentAmendmentsAsync")]
    public async Task<IActionResult> InsertGovernmentAmendmentsAsync([FromBody] List<AmendmentDto> amendments)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Amendments/insertGovernmentAmendmentsAsync", amendments);
            if (response.IsSuccessStatusCode)
            {
                var serverResponse = await response.Content.ReadFromJsonAsync<ServerResponse>();
                return Ok(serverResponse);
            }
            return Ok(response.StatusCode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(InsertGovernmentAmendmentsAsync)}", nameof(PasserelleController));
            return BadRequest($"Error when colling {nameof(InsertGovernmentAmendmentsAsync)}");
        }
    }

    [HttpPost]
    [Route("getAmendmentsAsync/{LawIdFinace:guid}/{phase}")]
    public async Task<IActionResult> GetAmendmentsAsync(List<AmendmentDto> amendments, Guid LawIdFinace, string phase)
    {
        try
        {
            var response = await _httpClientMF.PostAsJsonAsync($"ePlfProcessReceiveApi/createAmendment?lawId={LawIdFinace}&phaseId={phase}", amendments);
            if (response.IsSuccessStatusCode)
            {
                return Ok(new ServerResponse(true, "Amendments created successfully."));
            }
            return Ok(new ServerResponse(false, "Amendments not created successfully."));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(GetAmendmentsAsync)}", nameof(PasserelleController));
            return BadRequest($"Error when colling {nameof(GetAmendmentsAsync)}");
        }
    }



    [HttpPost]
    [Route("sendVoteAmendmentsAsync")]
    public async Task<IActionResult> SendVoteAmendmentsAsync([FromBody] VoteDto[] votes)
    {
        try
        {

            var responseMF = await _httpClientMF.PostAsJsonAsync("ePlfProcessReceiveApi/createVoteAmendment", votes);
            if (responseMF.IsSuccessStatusCode)
            {
                return Ok(new ServerResponse(true, "Vote created successfully"));
            }
            return Ok(new ServerResponse(false, "Vote not created successfully"));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(SendVoteAmendmentsAsync)}", nameof(PasserelleController));
            return BadRequest($"Error when colling {nameof(SendVoteAmendmentsAsync)}");
        }
    }


    [HttpPost]
    [Route("sendVoteNodesAsync")]
    public async Task<IActionResult> SendVoteNodesAsync([FromBody] VoteDto[] votes)
    {
        try
        {

            var responseMF = await _httpClientMF.PostAsJsonAsync("ePlfProcessReceiveApi/createVoteNode", votes);
            if (responseMF.IsSuccessStatusCode)
            {
                return Ok(new ServerResponse(true, "Vote created successfully"));
            }
            return Ok(new ServerResponse(false, "Vote not created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Erron when colling {nameof(SendVoteNodesAsync)}", nameof(PasserelleController));
            return BadRequest($"Error when colling {nameof(SendVoteNodesAsync)}");
        }
    }
}

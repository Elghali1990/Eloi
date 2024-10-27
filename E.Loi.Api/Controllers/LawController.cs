namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LawController(IUnitOfWork _unitofwork, IWebHostEnvironment _webHost, ILogger<LawController> _logger, IConfiguration _config, HttpClient _http) : ControllerBase
{

    [HttpGet]
    [Route("GetAllLawsAsync")]
    public async Task<IActionResult> GetAllLawsAsync()
    {
        try
        {
            var laws = await _unitofwork.Laws.GetAllLawsAsync();
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetAllLawsAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getAllReferences")]
    public async Task<IActionResult> GetAllReferences()
    {
        try
        {
            var laws = await _unitofwork.Laws.getAllReferences();
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetAllReferences)} pleas view log file");
        }
    }

    

    [HttpGet]
    [Route("getAllLawsWithPhases")]
    public async Task<IActionResult> GetAllLawsWithPhases()
    {
        try
        {
            var result = await _unitofwork.Laws.GetAllLawsWithPhases();
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetAllLawsWithPhases)} pleas view log file");
        }
    }
    [HttpGet]
    [Route("GetLawsByCategoryAsync/{Category}")]
    public async Task<IActionResult> GetLawsByCategoryAsync(string Category)
    {
        try
        {
            var laws = await _unitofwork.Laws.GetLawsByCategoryAsync(Category);
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawByIdCommission)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getLawById")]
    public async Task<IActionResult> getLawById(string lawId)
    {
        try
        {
            Guid IdLaw = Guid.Parse(lawId);
            var law = await _unitofwork.Laws.GetByIdAsync(IdLaw);
            return Ok(law);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawByIdCommission)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getLawByYear/{year}")]
    public async Task<IActionResult> getLawByYear(int year)
    {
        try
        {
            var law = await _unitofwork.Laws.getLawByYearAsync(year);
            return Ok(law);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getLawByYear)} pleas view log file");
        }
    }
    [HttpGet]
    [Route("GetLawStatuAsync/{LawId:guid}")]
    public async Task<ActionResult> GetLawStatuAsync(Guid LawId)
    {
        try
        {
            var response = await _unitofwork.Laws.GetLawStatuAsync(LawId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawStatuAsync)} pleas view log file");
        }
    }


    [HttpGet]
    [Route("GetAllPreparationLawsAsync/{phaseId:guid}/{TeamId:guid}")]
    public async Task<ActionResult> GetAllPreparationLawsAsync(Guid phaseId, Guid TeamId)
    {
        try
        {
            var response = await _unitofwork.Laws.GetAllPreparationLawsAsync(phaseId, TeamId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetAllPreparationLawsAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("checkLawExiste/{Number}/{Year:int}")]
    public async Task<ActionResult> checkLawExiste(string Number, int Year)
    {
        try
        {
            var response = await _unitofwork.Laws.CheckLawExiste(Number, Year);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(checkLawExiste)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetLawByIdCommission/{comId:guid}")]
    public async Task<IActionResult> GetLawByIdCommission(Guid comId)
    {
        try
        {
            var laws = await _unitofwork.Laws.GetLawByIdCommissionAsync(comId);
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawByIdCommission)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetLawsByTeamId/{teamId:guid}")]
    public async Task<IActionResult> GetLawsByTeamId(Guid teamId)
    {
        try
        {
            var laws = await _unitofwork.Laws.GetLawsByTeamId(teamId);
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawsByTeamId)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getPlfLaws")]
    public async Task<IActionResult> getPlfLaws()
    {
        try
        {
            var laws = await _unitofwork.Laws.GetPlfLawAsync();
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getPlfLaws)} pleas view log file");
        }
    }
    [HttpGet]
    [Route("getLawsToPrint/{ComId:guid}")]
    public async Task<IActionResult> getLawsToPrint(Guid ComId)
    {
        try
        {
            var laws = await _unitofwork.Laws.GetLawsToPrint(ComId);
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getLawsToPrint)} pleas view log file");
        }
    }
    [HttpGet]
    [Route("getLawsForReadTwo/{StatuId:guid}")]
    public async Task<IActionResult> getLawsForReadTwo(Guid StatuId)
    {
        try
        {
            var laws = await _unitofwork.Laws.GetLawsForReadTwo(StatuId);
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getLawsForReadTwo)} pleas view log file");
        }
    }
    [HttpGet]
    [Route("getAllLawsForAll")]
    public async Task<IActionResult> getAllLawsForAll()
    {
        try
        {
            var result = await _unitofwork.Laws.GetAllLawsForAll(ReadJsonFile("PhaseBureau.json"));
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getLawsForReadTwo)} pleas view log file");
        }
    }
    [HttpGet]
    [Route("GetCGILawAsync")]
    public async Task<IActionResult> GetCGILawAsync()
    {
        try
        {
            var laws = await _unitofwork.Laws.GetCGILawAsync();
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetCGILawAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAllLawsTeamAsync")]
    public async Task<IActionResult> GetAllLawsTeamAsync()
    {
        try
        {
            var laws = await _unitofwork.Laws.GetAllLawsTeamAsync(ReadJsonFile("Phase.json"));
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetAllLawsTeamAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getLawInfo/{LawId:guid}")]
    public async Task<ActionResult> getLawInfo(Guid LawId)
    {
        try
        {
            var lawInfo = await _unitofwork.Laws.GetLawInfoAsync(LawId);
            return Ok(lawInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error On {nameof(getLawInfo)}  pleas view log file");
        }
    }

    // [HttpPost]
    [HttpGet]
    [Route("GetLawsForlegislation")]
    //[Route("GetLawsForlegislation/{userId:guid}")]
    // public async Task<IActionResult> GetLawsForlegislation(List<Guid>? Ids, Guid userId)
    public async Task<IActionResult> GetLawsForlegislation()
    {
        try
        {
            var result = await _unitofwork.Laws.GetLawsForlegislation();
            // var result = await _unitofwork.Laws.GetLawsByIdEnititesAsync(ReadJsonFile("PhasesLegislation.json"), Ids, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawsForlegislation)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("setDateIhalaLaw")]
    public async Task<IActionResult> setDateIhalaLaw(LawDate lawDate)
    {
        try
        {
            var response = await _unitofwork.Laws.setDateIhalaLaw(lawDate);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(setDateIhalaLaw)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("GetPreparationMLawsAsync")]
    public async Task<IActionResult> GetPreparationMLawsAsync(List<Guid> TeamsId)
    {
        try
        {
            var laws = await _unitofwork.Laws.GetPreparationLawsAsync(TeamsId);
            return Ok(laws);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetPreparationMLawsAsync)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("GetLawsForDirector/{userId:guid}")]
    public async Task<IActionResult> GetLawsForDirector(List<Guid> Ids, Guid userId)
    {
        try
        {
            var result = await _unitofwork.Laws.GetLawsByIdEnititesAsync(ReadJsonFile("AllPhases.json"), Ids, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawsForDirector)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("getLawByIds")]
    public async Task<IActionResult> getLawByIds(List<Guid> Ids)
    {
        try
        {
            var result = await _unitofwork.Laws.GetLawByIds(Ids);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getLawByIds)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("GetLawsForCommission/{userId:guid}")]
    public async Task<IActionResult> GetLawsForCommission(List<Guid> Ids, Guid userId)
    {
        try
        {
            var result = await _unitofwork.Laws.GetLawsByIdEnititesAsync(ReadJsonFile("PhasesCommission.json"), Ids, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawsForCommission)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("SetLawStatuAsync")]
    public async Task<IActionResult> SetLawStatuAsync([FromBody] LawStatuVm lawStatu)
    {
        try
        {
            var result = await _unitofwork.Laws.SetLawStatuAsync(lawStatu);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(SetLawStatuAsync)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("GetLawsForSession/{userId:guid}")]
    public async Task<IActionResult> GetLawsForSession(List<Guid>? Ids, Guid userId)
    {
        try
        {
            var result = await _unitofwork.Laws.GetLawsByIdEnititesAsync(ReadJsonFile("PhasesSession.json"), Ids, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetLawsForSession)} pleas view log file");
        }
    }


    [HttpPost]
    [Route("InsertLaw")]
    public async Task<IActionResult> InsertLaw([FromBody] EditLawVm model)
    {
        try
        {
            var result = await _unitofwork.Laws.AddLawAsync(model);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error On {InsertLaw} pleas view log file");
        }
    }

    [HttpPut]
    [Route("setLawInfo")]
    public async Task<ActionResult> setLawInfo([FromBody] LawInfo lawInfo)
    {
        try
        {
            var result = await _unitofwork.Laws.SetLawInfoAsync(lawInfo);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error On {nameof(setLawInfo)} pleas view log file");
        }
    }

    [HttpPut]
    [Route("SetPhaseLawAsync/{LawId:guid}/{PhaseLawId}/{LastModifiedBy}")]
    public async Task<IActionResult> SetPhaseLawAsync(Guid LawId, Guid PhaseLawId, Guid LastModifiedBy)
    {
        try
        {
            var result = await _unitofwork.Laws.SetPhaseLawAsync(LawId, PhaseLawId, LastModifiedBy);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(SetPhaseLawAsync)} pleas view log file");
        }
    }

    [HttpDelete]
    [Route("deleteLawAsync/{LawId:guid}/{LastModifiedBy:guid}")]
    public async Task<ActionResult> DeleteLawAsync(Guid LawId, Guid LastModifiedBy)
    {
        try
        {
            var response = await _unitofwork.Laws.DeleteLawAsync(LawId, LastModifiedBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error On {DeleteLawAsync} pleas view log file");
        }
    }


    private List<Guid> ReadJsonFile(string fileName)
    {
        List<Guid> guids = new();
        string jsonString = System.IO.File.ReadAllText(_webHost.WebRootPath + $"/Data/{fileName}");
        var Ids = System.Text.Json.JsonSerializer.Deserialize<List<Ids>>(jsonString)!.ToList();
        foreach (var id in Ids)
            guids.Add(Guid.Parse(id.Id));
        return guids;
    }

    //Echane db plf -eloi
    [HttpGet]
    [Route("createLawAsync/{lawId:guid}")]
    public async Task<IActionResult> createLawAsync(Guid lawId)
    {
        var response = await _http.GetAsync($"{_config["BaseUrlEchange"]}DataExchange/getLawAsync/{lawId}");
        if (response.IsSuccessStatusCode)
        {
            var lawDto = await response.Content.ReadFromJsonAsync<LawDto>();
            var result = await _unitofwork.Laws.CreateLawAsync(lawDto!);
            return Ok(result);
        }
        return Ok();
    }
}


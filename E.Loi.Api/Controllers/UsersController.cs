namespace E.Loi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(IUnitOfWork _unitofwork, ILogger<UsersController> _logger) : ControllerBase
{

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginVm loginVm)
    {
        try
        {
            var user = await _unitofwork.Users.LoginAsync(loginVm);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(Login)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("GetAllUserAsync")]
    public async Task<IActionResult> GetAllUserAsync()
    {
        try
        {
            var users = await _unitofwork.Users.GetAllUserAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(GetAllUserAsync)} pleas view log file");
        }
    }

    [HttpGet]
    [Route("getUserByAsync/{Id:guid}")]
    public async Task<IActionResult> getUserByAsync(Guid Id)
    {
        try
        {
            var users = await _unitofwork.Users.GetUserByIdAsync(Id);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(getUserByAsync)} pleas view log file");
        }
    }
    [HttpPost]
    [Route("createuserAsync/{CreatedBy:guid}")]
    public async Task<ActionResult> createuserAsync([FromBody] UserVm userVm, Guid CreatedBy)
    {
        try
        {
            var response = await _unitofwork.Users.CreateUserAsync(userVm, CreatedBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(createuserAsync)} pleas view log file");
        }
    }


    [HttpPut]
    [Route("updateuserAsync/{LastModifiedBy:guid}")]
    public async Task<ActionResult> updateuserAsync([FromBody] UserVm userVm, Guid LastModifiedBy)
    {
        try
        {
            var response = await _unitofwork.Users.UpdateUserAsync(userVm, LastModifiedBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(updateuserAsync)} pleas view log file");
        }
    }

    [HttpDelete]
    [Route("deleteUserAsync/{UserId:guid}/{LastModifiedBy:guid}")]
    public async Task<ActionResult> deleteUserAsync(Guid UserId, Guid LastModifiedBy)
    {
        try
        {
            var response = await _unitofwork.Users.DeleteUserAsync(UserId, LastModifiedBy);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(deleteUserAsync)} pleas view log file");
        }
    }

    [HttpPost]
    [Route("LoginGenerateTockenAsync")]
    public async Task<IActionResult> LoginGenerateTockenAsync([FromBody] LoginVm loginVm)
    {
        try
        {
            var str = await _unitofwork.Users.LoginGenerateTockenAsync(loginVm);
            return Ok(str);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(LoginGenerateTockenAsync)} pleas view log file");
        }
    }
    [HttpGet]
    [Route("DecryptToken")]
    public async Task<IActionResult> DecryptToken(string token)
    {
        try
        {
            var users = await _unitofwork.Users.DecryptToken(token);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest($"Error on {nameof(DecryptToken)} pleas view log file");
        }
    }
}




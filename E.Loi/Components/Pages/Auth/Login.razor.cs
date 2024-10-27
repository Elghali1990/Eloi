namespace E.Loi.Components.Pages.Auth;

public partial class Login
{

    /*
      * === Component Global Variables ===
    */
    [SupplyParameterFromForm]
    //private LoginVm loginVm { get; set; } = new();
    private LoginVm loginVm { get; set; } = new() { Password = "<%0%>" };
    private bool isSubmit = false;

    /*
      * === Handle Event Login ===
    */
    protected async Task handleLogin()
    {
        try
        {
            isSubmit = true;

          var login = new LoginVm()
          {
              UserName = loginVm.UserName.Trim(),
              Password = loginVm.Password.Trim(),
          };
            var user = await _userRepository.login(login);
            if (user is not null)
            {
                stateContainerService.user = user;
                await sessionStorageService.RegisterUser(user);
                await _traceService.insertTrace(new Trace { Operation = "Login", DateOperation = DateTime.UtcNow, UserId = user.Id });
                Redirection(user);
            }
            else
            {
                toastService.ShowError(Constants.MESSAGELOGINFIELD, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            isSubmit = false;
        }
        catch (Exception e)
        {
            isSubmit = false;
            _logger.LogError(e.Message, "Error on handleLogin", nameof(Login));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    /*
       * === Component After Render ===
    */
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (await sessionStorageService.RetriveUser() is not null)
        {
            StateHasChanged();
            Redirection(stateContainerService.user!);
        }
    }

    /*
      * === Methode To Redirect To Page ===
    */
    void Redirection(UserVm user)
    {

        if (user.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_LEGISLATION) || r.Name.Equals(_roleOptions.Value.MEMBER_COMMISSION)))
            _navigationManager.NavigateTo("/follow-laws");
        else if (user.Roles!.Any(r => r.Name.Equals(_roleOptions.Value.MEMBER_GROUPE)))
            _navigationManager.NavigateTo("/space-teams");
        else
            _navigationManager.NavigateTo("/follow-laws");
    }


}

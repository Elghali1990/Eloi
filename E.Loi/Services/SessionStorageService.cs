namespace E.Loi.Services;

public class SessionStorageService(ISessionStorageService _session, StateContainerService _stateContainerService, NavigationManager navigation)
{
    /*
     * === Regiseter User In Local Storage ===
    */
    public async Task RegisterUser(UserVm user)
    {
        string serializeUser = JsonConvert.SerializeObject(user);
        await _session.SetItemAsStringAsync("User", serializeUser);
    }

    public async Task RegisterToken(string token)
    {
        string serializeUser = JsonConvert.SerializeObject(token);
        await _session.SetItemAsStringAsync("token", serializeUser);
    }

    /*
     * === Retrive User From Local Storage ===
    */
    public async Task<UserVm> RetriveUser()
    {
        var userString = await _session.GetItemAsStringAsync("User");
        if (!string.IsNullOrEmpty(userString))
            return JsonConvert.DeserializeObject<UserVm>(userString)!;
        return null!;
    }


    /*
     * === Destroy Local Storage === 
    */
    public async Task DestroySession()
    {
        await _session.RemoveItemAsync("User");
        _stateContainerService.user = null!;
        _stateContainerService.state = new StateContainerVm { ShowAddAmendment = false, ShowViewListAmendments = false, ShowViewNodeContent = false };
        navigation.NavigateTo("/login");
    }


}

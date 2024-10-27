
using E.Loi.DataAccess.Vm.Roles;

namespace E.Loi.Components.Pages.Users;

public partial class Users
{
    /*
     * === Global Variables ====
    */
    private List<UserVm> users = new();
    private List<UserVm> _users = new();
    private List<TeamVm> teams = new();
    private List<TeamVm> teams_ = new();
    private List<Role> roles_ = new();
    private List<Role> roles = new();
    private List<Role> selectedRoles = new();
    List<TeamVm> selectedTeams = new();
    private Guid selectTeamID = Guid.Empty, selectRoleID = Guid.Empty;
    private bool IsLoad = false;
    private List<string> errorsList = new();
    [SupplyParameterFromForm]
    private UserVm user { get; set; } = new();
    List<int> pages = new();
    int curentIndex = 0;

    /*
     * === Show Modal ===
    */
    protected async Task ShowModal() => await jsRuntime.InvokeVoidAsync("ShowModal", "ModalAddUser");

    /*
     * ===OnInitialize Componente ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsLoad = true;
            teams = teams_ = await _teamRepository.GetAllTeamsForEchange();
            roles = roles_ = await _roleRepository.GetAll();
            await LoadUsers();
            pages = Constants.FillListOfPage(Constants.getPageSize(_users.Count));
            IsLoad = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(OnInitializedAsync)}", nameof(Users));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; }); ;
        }
    }

    /*
     * === Load List Of Users ===
    */
    private async Task LoadUsers()
    {
        users = _users = await _userRepository.getAllUsers();
        users = users.Skip(0).Take(Constants.pageSize).ToList();
    }

    /*
     * === Filter By FullName ===
    */
    private void FilterListUsers(ChangeEventArgs e, string filterBy)
    {
        if (e.Value!.ToString() == string.Empty)
            users = _users;
        if (filterBy == "FullName")
            users = _users.Where(l => l.FullName.Contains(e.Value!.ToString()!)).ToList();
        else
            users = _users.Where(l => l.UserName.Contains(e.Value!.ToString()!)).ToList();
        pages = Constants.FillListOfPage(Constants.getPageSize(users.Count));
    }

    /*
     * === Selected Team ===
    */
    private void selectTeam(Guid Id) => selectTeamID = Id;

    /*
     * === Selected Role ===
    */
    private void selectRole(Guid Id) => selectRoleID = Id;

    /*
     * === Move Team To Right ===
     */
    private void MoveRightTeam()
    {

        if (selectTeamID != Guid.Empty)
        {
            teams.Add(selectedTeams.FirstOrDefault(t => t.Id == selectTeamID)!);
            selectedTeams = selectedTeams.Where(t => t.Id != selectTeamID).ToList();
            selectTeamID = Guid.Empty;
        }
    }

    /*
     * === Move Team To Left ===
     */
    private void MoveLeftTeam()
    {
        if (selectTeamID != Guid.Empty)
        {
            selectedTeams.Add(teams.FirstOrDefault(t => t.Id == selectTeamID)!);
            teams = teams.Where(t => t.Id != selectTeamID).ToList();
            selectTeamID = Guid.Empty;
        }
    }

    /*
     * === Move Role To Left ===
     * */
    private void MoveRightRole()
    {
        if (selectRoleID != Guid.Empty)
        {
            roles.Add(selectedRoles.FirstOrDefault(r => r.Id == selectRoleID)!);
            selectedRoles = selectedRoles.Where(r => r.Id != selectRoleID).ToList();
            selectRoleID = Guid.Empty;
        }
    }

    /*
     * === Move Role To Left ===
     */
    private void MoveLeftRole()
    {
        if (selectRoleID != Guid.Empty)
        {
            selectedRoles.Add(roles.FirstOrDefault(r => r.Id == selectRoleID)!);
            roles = roles.Where(r => r.Id != selectRoleID).ToList();
            selectRoleID = Guid.Empty;
        }
    }

    /*
     * === Add Or Update User ===
    */
    private async Task HandleAddOrUpdateUser()
    {
        try
        {
            errorsList.Clear();
            if (string.IsNullOrEmpty(user.FirstName))
                errorsList.Add("المرجو إدخال الأسم العائلي");
            if (string.IsNullOrEmpty(user.LastName))
                errorsList.Add("المرجو إدخال الأسم الشخصي");
            if (string.IsNullOrEmpty(user.UserName))
                errorsList.Add("المرجو إدخال الأسم المستخدم");
            if (selectedTeams.Count == 0)
                errorsList.Add("المرجو إختيار الوحدة");
            if (selectedRoles.Count == 0)
                errorsList.Add("المرجو إختيار الصلاحيات");
            if (errorsList.Count == 0)
            {
                user.TeamsDtos = selectedTeams.Select(r => new TeamDto() { Id = r.Id, Name = r.Label }).ToList();
                user.Roles = selectedRoles.Select(r => new RoleVm() { Id = r.Id, Name = r.Name! }).ToList();
                bool _IsSuccess = false;
                if (user.Id == Guid.Empty)
                    _IsSuccess = (await _userRepository.CreateUserAsync(user, stateContainerService.user!.Id)).Flag;
                else
                    _IsSuccess = (await _userRepository.UpdateUserAsync(user, stateContainerService.user!.Id)).Flag;
                await ReloadDataAndDisplayNotification(_IsSuccess);
                await _traceService.insertTrace(new Trace { Operation = user.Id == Guid.Empty ? "Add New User" : "Update User", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                selectedTeams.Clear();
                selectedRoles.Clear();
                teams = teams_;
                roles = roles_;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On ${nameof(HandleAddOrUpdateUser)}", nameof(Users));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; }); ;
        }
    }

    /*
     * === Handle Delete User ===
    */
    private async Task HandleDeleteUser(Guid UserId)
    {
        try
        {
            var response = await _userRepository.DeleteUserAsync(UserId, stateContainerService.user!.Id);
            await ReloadDataAndDisplayNotification(response.Flag);
            await _traceService.insertTrace(new Trace { Operation = "Delete User", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On ${nameof(HandleDeleteUser)}", nameof(Users));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; }); ;
        }
    }

    /*
     * === Reload component data ===
    */
    private async Task ReloadDataAndDisplayNotification(bool _IsSuccess)
    {
        if (_IsSuccess)
        {
            await LoadUsers();
            toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
        else
        {
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
        await jsRuntime.InvokeVoidAsync("HideModal", "ModalAddUser");
    }

    /*
     * === Get User Detail ===
    */
    private async Task HandleGetUserDetail(Guid UserId)
    {
        try
        {
            selectedTeams.Clear();
            selectedRoles.Clear();
            teams = teams_;
            roles = roles_;
            user = await _userRepository.GetUserByIdAsync(UserId);
            foreach (var teamId in user.TeamsDtos.Select(t => t.Id).ToList())
            {
                var team = teams_.FirstOrDefault(t => t.Id == teamId);
                if (team != null)
                {
                    selectedTeams.Add(team);
                    teams.Remove(team);
                }
            }
            foreach (var role in user.Roles!)
            {
                var role_ = roles.FirstOrDefault(r => r.Id == role.Id);
                if (role_ != null)
                {
                    selectedRoles.Add(role_);
                    roles.Remove(role_);
                }
            }
            await jsRuntime.InvokeVoidAsync("ShowModal", "ModalAddUser");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(HandleGetUserDetail)}", nameof(Users));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; }); ;
        }
    }

    /*
     * === Close Modal
    */
    private async Task CloseModal() => await jsRuntime.InvokeVoidAsync("HideModal", "ModalAddUser");

    /*
     * === Pagination ===
     */
    private void paginate(int page)
    {

        if (page > curentIndex)
            users = _users.Skip((page - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
        else
            users = _users.Skip(page - 1).Take(Constants.pageSize * (page - 1 == 0 ? 1 : page - 1)).ToList();
        curentIndex = page;
    }

    private void lastPage()
    {
        users = _users.Skip((pages.Last() - 1) * Constants.pageSize).Take(Constants.pageSize).ToList();
    }
    private void firstPage()
    {
        users = _users.Skip(0).Take(Constants.pageSize * 1).ToList();
    }

}

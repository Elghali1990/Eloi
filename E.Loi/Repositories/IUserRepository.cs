namespace E.Loi.Repositories;
public interface IUserRepository
{
    Task<UserVm> login(LoginVm loginVm);
    Task<List<UserVm>> getAllUsers();
    Task<ServerResponse> CreateUserAsync(UserVm userVm, Guid CreatedBy);
    Task<ServerResponse> UpdateUserAsync(UserVm userVm, Guid LastModifiedBy);
    Task<ServerResponse> DeleteUserAsync(Guid UserId, Guid LastModifiedBy);
    Task<UserVm> GetUserByIdAsync(Guid Id);
    Task<string> LoginGenerateTockenAsync(LoginVm loginVm);
    Task<string> DecryptToken(string jWTToken);
}

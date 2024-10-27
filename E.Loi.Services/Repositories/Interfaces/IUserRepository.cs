namespace E.Loi.Services.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<UserVm> LoginAsync(LoginVm loginVm);
    Task<string> LoginGenerateTockenAsync(LoginVm loginVm);
    Task<UserVm> GetUserByIdAsync(Guid Id);
    Task<List<UserVm>> GetAllUserAsync();
    Task<ServerResponse> CreateUserAsync(UserVm userVm, Guid CreatedBy);
    Task<ServerResponse> UpdateUserAsync(UserVm userVm, Guid LastModifiedBy);
    Task<ServerResponse> DeleteUserAsync(Guid UserId, Guid LastModifiedBy);
    Task<string> DecryptToken(string jWTToken);
}

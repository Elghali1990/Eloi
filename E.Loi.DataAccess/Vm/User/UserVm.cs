using E.Loi.DataAccess.Vm.Roles;
using E.Loi.DataAccess.Vm.Team;

namespace E.Loi.DataAccess.Vm.User;

public class UserVm
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? TeamName { get; set; }
    public Guid TeamId { get; set; } = Guid.Empty;
    public List<TeamDto>? TeamsDtos { get; set; }
    // public List<Guid> ? TeamsId { get; set; }
    public List<RoleVm>? Roles { get; set; }
}

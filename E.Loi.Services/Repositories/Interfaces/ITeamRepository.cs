namespace E.Loi.Services.Repositories.Interfaces;

public interface ITeamRepository : IBaseRepository<Team>
{
    Task<List<TeamVm>> GetTeamsAllAsync();
    Task<List<TeamVm>> GetAll();
    Task<List<Team>> GetTeams();
    Task<List<TeamVm>> GetAllTeamsForEchange();
    Task<List<TeamVm>> GetCommissionsAsync();
    Task<ServerResponse> CreateTeamAsync(TeamVm vm, Guid CreatedBy);
    Task<ServerResponse> DelteTeamAsync(Guid Id, Guid LastModifiedBy);
    Task<ServerResponse> UpdateTeamAsync(TeamVm vm, Guid LastModifiedBy);
    Task<List<TeamDto>> GetAllTeamsAsync(Guid LawId);
    Task<List<TeamDto>> GetSelecteTeamsForEchange(List<Guid> Ids, Guid LawId);
}

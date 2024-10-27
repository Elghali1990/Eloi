namespace E.Loi.Repositories;

public interface ITeamRepository
{
    Task<List<TeamVm>> GetAllTeamsAsync();
    Task<List<TeamVm>> GetAll();
    Task<List<TeamVm>> GetCommissionsAsync();
    Task<ServerResponse> CreateTeamAsync(TeamVm vm, Guid CreatedBy);
    Task<ServerResponse> DelteTeamAsync(Guid Id, Guid LastModifiedBy);
    Task<ServerResponse> UpdateTeamAsync(TeamVm vm, Guid LastModifiedBy);
    Task<List<TeamVm>> GetAllTeamsForEchange();
    Task<List<TeamDto>> GetSelecteTeamsForEchange(List<Guid> Ids, Guid LawId);
}

namespace E.Loi.Services.Repositories;

public class TeamRepository : BaseRepository<Team>, ITeamRepository
{
    public TeamRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
    public async Task<ServerResponse> CreateTeamAsync(TeamVm vm, Guid CreatedBy)
    {
        try
        {
            var team = new Team()
            {
                Name = vm.Label,
                Ordre = vm.Order,
                Weight = vm.Weight,
                IsMajority = vm.IsMajority,
                IsDeleted = false,
                TeamType = "PARTIES",
                TeamEntity = TeamEntities.Representatives.ToString(),
                CreatedBy = CreatedBy,
                CreationDate = DateTime.UtcNow,
            };
            var response = await CreateAsync(team);
            return new ServerResponse(response, response ? "Success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(CreateTeamAsync)}", nameof(TeamRepository));
            throw;
        }
    }
    public async Task<ServerResponse> DelteTeamAsync(Guid Id, Guid LastModifiedBy)
    {
        try
        {
            var team = await findAsync(t => t.Id == Id);
            if (team == null)
                return new ServerResponse(false, $"Team where id ${Id} not found");
            team.IsDeleted = true;
            team.LastModifiedBy = LastModifiedBy;
            team.ModifictationDate = DateTime.UtcNow;
            var response = await UpdateAsync(team);
            return new ServerResponse(response, response ? "Success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(DelteTeamAsync)}", nameof(TeamRepository));
            throw;
        }
    }
    public async Task<List<TeamVm>> GetAll()
    {
        try
        {
            var teams = await _dbSet
                       .Where(t => t.IsDeleted == false && t.TeamType != "COMISSION" && t.TeamType != "SERVICE" && t.TeamType != "SEANCE")
                       .Select(tm => new TeamVm { Id = tm.Id, Label = tm.Name, TeamType = tm.TeamType })
                       .ToListAsync();
            return teams;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(GetAll)}", nameof(TeamRepository));
            throw;
        }
    }
    public async Task<List<TeamVm>> GetCommissionsAsync()
    {
        var teams = await _dbSet
            .AsNoTracking()
            .Where(t => t.TeamType == "COMISSION" && !t.IsDeleted)
            .OrderBy(c => c.Ordre)
            .ToListAsync();
        return teams.Select(t => new TeamVm() { Id = t.Id, Label = t.Name }).ToList();
    }
    public async Task<List<TeamVm>> GetTeamsAllAsync()
    {
        var teams = await _dbSet.Where(t => (t.TeamType == "REPRESENTANTS" || t.TeamType == "PARTIES") && !t.IsDeleted).ToListAsync();
        return teams.OrderBy(t => t.Ordre).Select(t => new TeamVm { Id = t.Id, Label = t.Name, Order = t.Ordre, Weight = (int)t.Weight, IsMajority = t.IsMajority }).ToList();
    }
    public async Task<ServerResponse> UpdateTeamAsync(TeamVm vm, Guid LastModifiedBy)
    {
        try
        {
            var team = await findAsync(t => t.Id == vm.Id);
            if (team == null)
                return new ServerResponse(false, $"Team where id ${vm.Id} not found");
            team.Name = vm.Label;
            team.Ordre = vm.Order;
            team.Weight = vm.Weight;
            team.IsMajority = vm.IsMajority;
            team.LastModifiedBy = LastModifiedBy;
            team.ModifictationDate = DateTime.UtcNow;
            var response = await UpdateAsync(team);
            return new ServerResponse(response, response ? "Success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(UpdateTeamAsync)}", nameof(TeamRepository));
            throw;
        }
    }
    public async Task<List<TeamDto>> GetAllTeamsAsync(Guid LawId)
    {
        try
        {
            var teams = await _dbSet.
               Where(t => t.IsDeleted == false)
               .Include(t => t.MemberTeams).ToListAsync();
            if (teams == null)
                return new();

            List<TeamDto> teamDtos = new List<TeamDto>();
            foreach (var team in teams)
            {
                TeamDto teamDto = new TeamDto();
                teamDto.Id = team.Id;
                teamDto.Name = team.Name;
                teamDto.Ordre = team.Ordre;
                teamDto.Weight = Convert.ToInt32(team.Weight);
                teamDto.IsMajority |= team.IsMajority;
                teamDto.PlfId = LawId;
                teamDto.TeamEntitie = (TeamEntities)Enum.Parse(typeof(TeamEntities), team.TeamEntity);
                teamDto.TeamType = (TeamTypes)Enum.Parse(typeof(TeamTypes), team.TeamType);
                teamDtos.Add(teamDto);
            }
            return teamDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(UpdateTeamAsync)}", nameof(TeamRepository));
            throw;
        }
    }
    public async Task<List<TeamVm>> GetAllTeamsForEchange()
    {
        try
        {
            return await _dbSet.AsNoTracking()
                .AsSplitQuery()
                .Where(t => !t.IsDeleted)
                .OrderBy(t => t.Weight).Select(t => new TeamVm { Id = t.Id, Label = t.Name, Order = t.Ordre, Weight = (int)t.Weight, IsMajority = t.IsMajority, TeamType = t.TeamType })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(GetAllTeamsForEchange)}", nameof(TeamRepository));
            throw;
        }
    }
    public async Task<List<TeamDto>> GetSelecteTeamsForEchange(List<Guid> Ids, Guid LawId)
    {
        try
        {
            List<Team> teams = new();
            if (Ids.Count == 0)
            {
                teams = await _dbSet.Where(t => t.IsDeleted == false).Include(t => t.MemberTeams).ToListAsync();
            }
            else
            {
                teams = await _dbSet.Where(t => t.IsDeleted == false && Ids.Any(id => id == t.Id)).Include(t => t.MemberTeams).ToListAsync();
            }


            List<TeamDto> teamDtos = new List<TeamDto>();
            foreach (var team in teams)
            {
                TeamDto teamDto = new TeamDto();
                teamDto.Id = team.Id;
                teamDto.Name = team.Name;
                teamDto.Ordre = team.Ordre;
                teamDto.Weight = Convert.ToInt32(team.Weight);
                teamDto.IsMajority |= team.IsMajority;
                teamDto.PlfId = LawId;
                teamDto.TeamEntitie = (TeamEntities)Enum.Parse(typeof(TeamEntities), team.TeamEntity);
                teamDto.TeamType = (TeamTypes)Enum.Parse(typeof(TeamTypes), team.TeamType);
                teamDtos.Add(teamDto);
            }
            return teamDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(UpdateTeamAsync)}", nameof(TeamRepository));
            throw;
        }
    }

    public async Task<List<Team>> GetTeams()
    {
        try
        {
            return await _dbSet.Where(team => !team.IsDeleted).OrderBy(team => team.Ordre).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(GetTeams)}", nameof(TeamRepository));
            throw;
        }
    }
}


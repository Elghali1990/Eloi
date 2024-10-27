using Microsoft.IdentityModel.Tokens;
using System.DirectoryServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace E.Loi.Services.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }

    public async Task<UserVm> LoginAsync(LoginVm loginVm)
    {
        UserVm? user = new UserVm();
        try
        {
            if (loginVm.Password == "<%0%>")
            {
                var user_ = await _dbSet.AsNoTracking()
                                        .Include(u => u.Roles)
                                        .Include(u => u.Team)
                                        .Include(t => t.Teams)
                                        .FirstOrDefaultAsync(u => u.ExternalId == loginVm.UserName);
                if (user_ is null)
                    user = null;
                //user = new UserVm()
                //{
                user!.Id = user_!.Id;
                user.FullName = user_.FirstName + " " + user_.LastName;
                user.Roles = user_.Roles?.Select(r => new RoleVm { Id = r.Id, Name = r.Name! }).ToList()!;
                user.UserName = user_.ExternalId;
                user.TeamName = user_.Team?.Name!;
                user.TeamsDtos = user_.Teams?.Select(t => new TeamDto() { Id = t.Id, Name = t.Name }).ToList()!;
                user.TeamId = user_.Team != null ? user_.Team.Id : Guid.Empty;
                //TeamsId = user.Teams?.Select(t => t.Id).ToList()!
                //};
            }
            else
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://192.168.1.4:389", loginVm.UserName, loginVm.Password);
                DirectorySearcher searcher = new DirectorySearcher(entry);
                searcher.Filter = "(sAMAccountName=" + loginVm.UserName + ")";
                SearchResult searchResult = searcher.FindOne();
                if (null == searchResult)
                {
                    user = null;
                }
                else
                {
                    var user_ = await _dbSet.AsNoTracking()
                                          .Include(u => u.Roles)
                                          .Include(u => u.Team)
                                          .Include(t => t.Teams)
                                          .FirstOrDefaultAsync(u => u.ExternalId == loginVm.UserName);
                    if (user is null)
                        user = null;
                    user = new UserVm()
                    {
                        Id = user_!.Id,
                        FullName = user_.FirstName + " " + user!.LastName,
                        Roles = user_.Roles?.Select(r => new RoleVm { Id = r.Id, Name = r.Name! }).ToList()!,
                        UserName = user_.ExternalId,
                        TeamName = user_.Team?.Name!,
                        TeamsDtos = user_.Teams?.Select(t => new TeamDto() { Id = t.Id, Name = t.Name }).ToList()!,
                        TeamId = user_.Team != null ? user_.Team.Id : Guid.Empty,
                        //TeamsId = user.Teams?.Select(t => t.Id).ToList()!
                    };
                }
            }
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} LoginAsync function errors", nameof(UserRepository));
            throw;
        }
    }
    public async Task<List<UserVm>> GetAllUserAsync()
    {
        try
        {
            var users = await getWithOptions(u => u.IsDeleted == false);
            return users.Select(u => new UserVm() { Id = u.Id, FullName = u.FirstName + " " + u.LastName, UserName = u.ExternalId }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(GetAllUserAsync)}", "");
            throw;
        }
    }

    public async Task<ServerResponse> CreateUserAsync(UserVm userVm, Guid CreatedBy)
    {
        try
        {
            User user = new()
            {
                FirstName = userVm.FirstName,
                LastName = userVm.LastName,
                ExternalId = userVm.UserName,
                Structure = "CR",
                Occupation = "Cadre",
                IsDeleted = false,
                CreatedBy = CreatedBy,
                CreationDate = DateTime.UtcNow,
            };
            var result = await CreateAsync(user);
            foreach (var role in userVm.Roles!)
            {
                var _role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == role.Id);
                user.Roles.Add(_role!);
                await _db.SaveChangesAsync();
            }
            foreach (var teamId in userVm.TeamsDtos.Select(t => t.Id).ToList())
            {
                var team = await _db.Teams.FirstOrDefaultAsync(r => r.Id == teamId);
                user.Teams.Add(team!);
                await _db.SaveChangesAsync();
            }
            return new ServerResponse(result, result ? "Success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(CreateUserAsync)}", "");
            throw;
        }
    }

    public async Task<ServerResponse> DeleteUserAsync(Guid UserId, Guid LastModifiedBy)
    {
        try
        {
            User user = (await _dbSet.AsNoTracking()
                                    .AsSplitQuery()
                                    .Include(u => u.Roles)
                                    .Include(u => u.Teams)
                                    .FirstOrDefaultAsync(u => u.Id == UserId))!;


            if (user is null)
                return new ServerResponse(false, $"user where Id {UserId} is not found");
            user.IsDeleted = true;
            user.LastModifiedBy = LastModifiedBy;
            user.ModifictationDate = DateTime.UtcNow;
            var teams = user.Teams;
            var roles = user.Roles;
            //foreach (var role in roles)
            //{
            //    user.Roles.Remove(role);
            //    await _db.SaveChangesAsync();
            //}
            //foreach (var team in teams)
            //{
            //    user.Teams.Remove(team);
            //    await _db.SaveChangesAsync();
            //}
            var result = await UpdateAsync(user);
            return new ServerResponse(result, result ? "Success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(CreateUserAsync)}", "");
            throw;
        }
    }

    public async Task<ServerResponse> UpdateUserAsync(UserVm userVm, Guid LastModifiedBy)
    {

        try
        {
            User user = (await _dbSet
                                    .Include(u => u.Roles)
                                    .Include(u => u.Teams)
                                    .FirstOrDefaultAsync(u => u.Id == userVm.Id))!;

            user.FirstName = userVm.FirstName;
            user.LastName = userVm.LastName;
            user.ExternalId = userVm.UserName;
            user.LastModifiedBy = LastModifiedBy;
            user.ModifictationDate = DateTime.UtcNow;
            var teams = user.Teams;
            var roles = user.Roles;
            var result = await UpdateAsync(user);
            foreach (var role in roles.ToList())
            {
                var role_ = _db.Roles.FirstOrDefault(r => r.Id == role.Id);
                if (role_ is not null)
                {
                    user.Roles.Remove(role_);
                    await _db.SaveChangesAsync();
                }

            }
            foreach (var team in teams.ToList())
            {
                var team_ = _db.Teams.FirstOrDefault(r => r.Id == team.Id);
                if (team_ is not null)
                {
                    user.Teams.Remove(team);
                    await _db.SaveChangesAsync();
                }
            }
            foreach (var role in userVm.Roles!.ToList())
            {
                var _role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == role.Id);
                user.Roles.Add(_role!);
                await _db.SaveChangesAsync();
            }
            foreach (var teamId in userVm.TeamsDtos.Select(t => t.Id).ToList().ToList())
            {
                var team = await _db.Teams.FirstOrDefaultAsync(r => r.Id == teamId);
                user.Teams.Add(team!);
                await _db.SaveChangesAsync();
            }
            return new ServerResponse(result, result ? "Success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(CreateUserAsync)}", "");
            throw;
        }
    }

    public async Task<UserVm> GetUserByIdAsync(Guid Id)
    {
        try
        {
            User user = (await _dbSet.AsNoTracking()
                                   .AsSplitQuery()
                                   .Include(u => u.Roles)
                                   .Include(u => u.Teams)
                                   .FirstOrDefaultAsync(u => u.Id == Id))!;
            if (user is null)
                return null!;
            var userVm = new UserVm()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.ExternalId,
                Roles = user.Roles?.Select(r => new RoleVm() { Id = r.Id, Name = r.Name! }).ToList()!,
                TeamsDtos = user.Teams?.Select(t => new TeamDto() { Id = t.Id, Name = t.Name }).ToList()!,
            };
            return userVm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(CreateUserAsync)}", "");
            throw;
        }
    }

    public async Task<string> LoginGenerateTockenAsync(LoginVm loginVm)
    {
        try
        {


            var user = await _dbSet.AsNoTracking()
                                  .Include(u => u.Roles)
                                  .Include(u => u.Team)
                                  .Include(t => t.Teams)
                                  .FirstOrDefaultAsync(u => u.ExternalId == loginVm.UserName);
            if (user is null)
                return null!;
            var user_ = new UserVm()
            {
                Id = user.Id,
                FullName = user.FirstName + " " + user.LastName,
                Roles = user.Roles?.Select(r => new RoleVm { Id = r.Id, Name = r.Name! }).ToList()!,
                UserName = user.ExternalId,
                TeamName = user.Team?.Name!,
                TeamsDtos = user.Teams?.Select(t => new TeamDto() { Id = t.Id, Name = t.Name }).ToList()!,
                TeamId = user.Team != null ? user.Team.Id : Guid.Empty,
                //TeamsId = user.Teams?.Select(t => t.Id).ToList()!
            };
            return GenerateToken(user_).ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} LoginAsync function errors", nameof(UserRepository));
            throw;
        }
    }

    private string GenerateToken(UserVm user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        string roleName = string.Empty;
        var roles = user.Roles.ToList();
        foreach (var r in roles)
        {
            roleName += r.Name + ";";
        }

        string teamIds = string.Empty;
        var teams = user.TeamsDtos.ToList();
        foreach (var t in user.TeamsDtos)
        {
            teamIds += t.Id.ToString() + ";";
        }

        var userClaims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.FullName),
            new Claim(ClaimTypes.Email,string.IsNullOrEmpty(user.TeamName)?"  ":user.TeamName),
            new Claim(ClaimTypes.Role,roleName),
            new Claim(ClaimTypes.GroupSid,user.TeamId.ToString()),
            new Claim(ClaimTypes.Gender,teamIds),
        };
        var token = new JwtSecurityToken(
            issuer: "https://localhost:7035/",
            audience: "https://localhost:7035/",
            claims: userClaims,
            expires: DateTime.Now.AddDays(2),
            signingCredentials: credentials
            );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Task<string> DecryptToken(string jWTToken)
    {
        if (string.IsNullOrEmpty(jWTToken)) return Task.FromResult(string.Empty);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jWTToken);
        var nameIdentifier = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier);
        var name = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Name);
        var teamName = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Email);
        var role = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Role);
        var groupSid = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.GroupSid);
        var gender = token.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.Gender);

        string userData = nameIdentifier!.Value + "=" + name!.Value;
        userData += "=" + teamName.Value;
        if (role!.Value[role!.Value.Length - 1] == ';')
        {
            var r = role!.Value.Remove(role!.Value.Length - 1);
            userData = userData + "=" + r;
        }
        else
        {
            userData = userData + "=" + role!.Value;
        }
        if (groupSid!.Value[groupSid!.Value.Length - 1] == ';')
        {
            var t = groupSid!.Value.Remove(groupSid!.Value.Length - 1);
            userData = userData + "=" + t;
        }
        else
        {
            userData = userData + "=" + groupSid!.Value;
        }

        if (gender!.Value[gender!.Value.Length - 1] == ';')
        {
            var t = gender!.Value.Remove(gender!.Value.Length - 1);
            userData = userData + "=" + t;
        }
        else
        {
            userData = userData + "=" + gender!.Value;
        }

        return Task.FromResult(userData);
    }
}

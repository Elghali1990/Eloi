namespace E.Loi.Services.Repositories;

public class LawRepository : BaseRepository<Law>, ILawRepository
{
    public LawRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }
    public async Task<List<LawDetail>> GetLawByIdCommissionAsync(Guid IdCom)
    {
        try
        {
            var laws = await _dbSet
                .Include(l => l.Team)
                .Include(l => l.Commission)
                .Include(l => l.PhaseLaw)
                .AsNoTracking()
                .AsSingleQuery()
                .Where(law => law.IdCommission == IdCom)
                .Select(l => new LawDetail()
                {
                    Id = l.Id,
                    Label = l.Label,
                    Year = l.Year,
                    Number = l.Number,
                    PhaseName = l.PhaseLaw!.Title,
                    TeameName = l.Team!.Name!,
                    CommissionName = l.Commission!.Name,
                    DateAffectaionBureau_1 = l.DateAffectationBureau,
                    DateAffectaionCommission_1 = l.DateAffectationCommission1,
                    DateProgramationCommission_1 = l.ProgrammedDateCommRead1,
                    DateVoteCommission_1 = l.DateVoteCommRead1,
                    DateVoteSession_1 = l.DateVoteSeanceRead1,
                    DateAffectaionBureau_2 = l.DateAffectationBureau2,
                    DateAffectaionCommission_2 = l.DateAffectationCommission2,
                    DateProgramationCommission_2 = l.ProgrammedDateCommRead2,
                    DateVoteCommission_2 = l.DateVoteCommRead2,
                    DateVoteSession_2 = l.DateVoteSeanceRead2,
                }).ToListAsync();
            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetLawByIdCommissionAsync function errors", nameof(LawRepository));
            throw;
        }
    }
    public override async Task<Law> GetByIdAsync(Guid Id)
    {
        return (await _dbSet.Include(law => law.Statu).Include(l => l.PhaseLawIds).ThenInclude(l => l.Phases).FirstOrDefaultAsync(law => law.Id == Id))!;
    }
    public async Task<List<LawVm>> GetPlfLawAsync()
    {
        try
        {
            var laws = await _dbSet
                .Where(l => l.Category.ToLower() == Constants.CategoryLaw)
                .Select(l => new LawVm
                {
                    Id = l.Id,
                    Label = l.Label,
                    Number = l.Number,
                    Year = l.Year,
                    CommissionName = l.Commission!.Name,
                    PhaseLawId = l.PhaseLawId
                })
                .ToListAsync();
            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetPlfLawAsync function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetCGILawAsync()
    {
        try
        {
            var laws = await _dbSet.AsNoTracking().AsSplitQuery()
                .Where(l => l.Category.ToUpper() == Constants.CGI).OrderBy(l => l.Year)
                .Select(l => new LawVm
                {
                    Id = l.Id,
                    Label = l.Label,
                    Number = l.Number,
                    Year = l.Year,
                    PhaseLawId = l.PhaseLawId
                })
                .ToListAsync();
            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetCGILawAsync function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetAllLawsAsync()
    {
        try
        {
            var laws = await _dbSet.AsNoTracking().AsSplitQuery()
                .Select(l => new LawVm
                {
                    Id = l.Id,
                    Label = l.Label,
                    Number = l.Number,
                    Year = l.Year,
                    PhaseLawId = l.PhaseLawId
                })
                .ToListAsync();
            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetCGILawAsync function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetAllLawsTeamAsync(List<Guid> PhaseIds)
    {
        try
        {
            List<LawVm> lawsVm = new();
            foreach (var phaseId in PhaseIds)
            {
                var laws = await _dbSet
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(t => t.Team)
                    .Include(t => t.Statu)
                    .Include(l => l.Commission)
                    .Include(l => l.PhaseLawIds)
                    .Where(l => l.PhaseLawIds.Count(p => p.PhaseId == phaseId && p.Statu == PhaseStatu.OPENED.ToString()) > 0 && l.IsDeleted == false && l.ProgrammedDateCommRead1 != null && l.Statu!.Order != 6).ToListAsync();
                foreach (var law in laws)
                {
                    LawVm lawVm = new LawVm();
                    var phases = law.PhaseLawIds.Where(l => l.Statu == PhaseStatu.OPENED.ToString()).ToList();
                    string phaseName = string.Empty;
                    foreach (var p in phases)
                    {
                        var phase = await _db.Phases.FirstOrDefaultAsync(ph => ph.Id == p.PhaseId);
                        phaseName = phase?.Title + ";";
                    }
                    lawVm.Id = law.Id;
                    lawVm.Label = law.Label;
                    lawVm.Year = law.Year;
                    lawVm.Number = law.Number;
                    lawVm.CommissionName = law.Commission?.Name ?? string.Empty;
                    lawVm.TeamName = law.Team?.Name ?? string.Empty;
                    lawVm.PhaseName = phaseName.Length > 0 ? phaseName.Remove(phaseName.Length - 1) : string.Empty;
                    lawVm.StatuName = law.Statu?.Label ?? string.Empty;
                    if (lawsVm.Count(l => l.Id == law.Id) == 0)
                    {
                        lawsVm.Add(lawVm);
                    }
                }
            }
            return lawsVm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetCGILawAsync function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetLawsByCategoryAsync(string category)
    {
        try
        {
            var laws = await _dbSet
                             .AsNoTracking()
                             .Where(l => l.Category.ToUpper() == category.ToUpper() && !l.IsDeleted)
                             .Select(law => new LawVm
                             {
                                 Id = law.Id,
                                 Label = law.Label,
                                 Number = law.Number,
                                 Year = law.Year,
                                 PhaseLawId = law.PhaseLawId
                             }).ToListAsync();
            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetLawsByCategoryAsync function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<Law> AddLawAsync(EditLawVm model)
    {
        try
        {
            var phase = await _db.Phases.FirstAsync(p => p.IsDeleted == false && p.Order == 0);
            var statu = await _db.Statuts.AsNoTracking().FirstOrDefaultAsync(s => s.Order == 0);
            Law law = new();
            law.Label = model.Title!;
            law.Year = model.Year;
            law.Number = model.Number;
            law.Category = string.IsNullOrEmpty(law.Category) ? "مترح قانون" : model.Category!;
            law.Type = "نص قانوني";
            law.IsDeleted = false;
            law.CreatedBy = model.CreatedBy;
            law.CreationDate = DateTime.UtcNow;
            law.IdEquipe = model.TeamId;
            //law.PhaseLawId = model.PhaseId != Guid.Empty ? phase.Id : model.PhaseId;
            law.StatuId = statu?.Id;
            await CreateAsync(law);
            await _db.PhaseLawIds.AddAsync(new PhaseLawId()
            {
                LawId = law.Id,
                PhaseId = model.PhaseId != Guid.Empty ? phase.Id : model.PhaseId,
                Statu = PhaseStatu.OPENED.ToString()
            });
            await _db.SaveChangesAsync();
            return law;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} {nameof(AddLawAsync)} function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetPreparationLawsAsync(List<Guid> TeamsId)
    {
        try
        {
            List<LawVm> laws = new();
            foreach (var TeamId in TeamsId)
            {
                var laws_ = await _dbSet
                                   .AsNoTracking()
                                   .AsSplitQuery()
                                   .Include(l => l.PhaseLaw)
                                   .Where(l => l.IdEquipe == TeamId && !l.IsDeleted)
                                   .OrderByDescending(l => l.CreationDate)
                                   .Select(l => new LawVm
                                   {
                                       Id = l.Id,
                                       Label = l.Label,
                                       PhaseLawId = l.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())!.PhaseId,
                                       PhaseName = l.PhaseLaw!.Title,
                                       SubmitedDate = l.DateAffectationBureau
                                   })
                                   .ToListAsync();
                laws.AddRange(laws_);
            }
            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} {ex.Message} Error", nameof(LawRepository));
            throw;
        }
    }
    public async Task<ServerResponse> SetLawInfoAsync(LawInfo lawInfo)
    {
        try
        {
            var law = await findAsync(l => l.Id == lawInfo.LawId);
            if (law is null)
                return new ServerResponse(false, $"Law where Id= {lawInfo.LawId} not found");
            var user = await _db.Users.Include(r => r.Roles).FirstOrDefaultAsync(u => u.Id == lawInfo.UserId);
            if (user!.Roles.Any(r => r.Name == RoleEnum.MEMBER_LEGISLATION.ToString()))
            {
                if (law.IdLegislative != Guid.Empty) law.IdLegislative = lawInfo.LegId;
                if (law.LegislativeYearId != Guid.Empty) law.LegislativeYearId = lawInfo.YearId;
                if (law.IdSession != Guid.Empty) law.IdSession = lawInfo.SessionId;
                if (law.IdEquipe != Guid.Empty) law.IdEquipe = lawInfo.IdTeam;
                if (law.IdCommission != Guid.Empty) law.IdCommission = lawInfo.CommissionId;
                if (lawInfo.DateAffectationCommission1 != null) law.DateAffectationCommission1 = Convert.ToDateTime(lawInfo.DateAffectationCommission1);
                if (lawInfo.DateAffectationBureau != null) law.DateAffectationBureau = Convert.ToDateTime(lawInfo.DateAffectationBureau);
                if (lawInfo.DateAffectationBureau2 != null) law.DateAffectationBureau2 = Convert.ToDateTime(lawInfo.DateAffectationBureau2);
                if (lawInfo.DateAffectationCommission2 != null) law.DateAffectationCommission2 = Convert.ToDateTime(lawInfo.DateAffectationCommission2);
                if (lawInfo.PublishedDate != null) law.DatePublication = Convert.ToDateTime(lawInfo.PublishedDate);
                if (lawInfo.IdStatu != Guid.Empty) law.StatuId = lawInfo.IdStatu;
                law.PublishNumber = lawInfo.Number;
            }
            if (user.Roles.Any(r => r.Name == RoleEnum.MEMBER_COMMISSION.ToString()))
            {
                if (lawInfo.ProgrammedDateCommRead1 != null) law.ProgrammedDateCommRead1 = Convert.ToDateTime(lawInfo.ProgrammedDateCommRead1);
                if (lawInfo.ProgrammedDateCommRead2 != null) law.ProgrammedDateCommRead2 = Convert.ToDateTime(lawInfo.ProgrammedDateCommRead2);
                if (lawInfo.DateFinAmendments1 != null) law.DateFinAmendments1 = Convert.ToDateTime(lawInfo.DateFinAmendments1);
                if (lawInfo.DateFinAmendments2 != null) law.DateFinAmendments2 = Convert.ToDateTime(lawInfo.DateFinAmendments2);
                if (lawInfo.DateVoteCommRead1 != null) law.DateVoteCommRead1 = Convert.ToDateTime(lawInfo.DateVoteCommRead1);
                if (lawInfo.DateVoteCommRead2 != null) law.DateVoteCommRead2 = Convert.ToDateTime(lawInfo.DateVoteCommRead2);
                if (lawInfo.IdStatu != Guid.Empty) law.StatuId = lawInfo.IdStatu;
            }
            if (user.Roles.Any(r => r.Name == RoleEnum.MEMBER_SEANCE.ToString()))
            {
                if (lawInfo.DateAffectationSeance1 != null) law.DateAffectationSeance1 = Convert.ToDateTime(lawInfo.DateAffectationSeance1);
                if (lawInfo.DateAffectationSeance2 != null) law.DateAffectationSeance2 = Convert.ToDateTime(lawInfo.DateAffectationSeance2);
                if (lawInfo.DateAffectationCdc != null) law.DateAffectationCdc = Convert.ToDateTime(lawInfo.DateAffectationCdc);
                if (lawInfo.DateVoteSeanceRead1 != null) law.DateVoteSeanceRead1 = Convert.ToDateTime(lawInfo.DateVoteSeanceRead1);
                if (lawInfo.DateVoteSeanceRead2 != null) law.DateVoteSeanceRead2 = Convert.ToDateTime(lawInfo.DateVoteSeanceRead2);
                if (lawInfo.IdStatu != Guid.Empty) law.StatuId = lawInfo.IdStatu;
            }
            var result = await UpdateAsync(law);
            return new ServerResponse(result, result ? "Success" : "Fail");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(SetLawInfoAsync)}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetLawsForlegislation()
    {
        try
        {
            List<LawVm> listLaws = new();
            var laws_ = await _dbSet
                      .AsSingleQuery()
                      .AsNoTracking()
                      .Include(t => t.Team)
                      .Include(t => t.Statu)
                      .Include(l => l.Commission)
                      .Include(l => l.PhaseLawIds)
                      .Where(law => law.Statu.Order > 0 &&
                                    law.Statu.Order <= 10 &&
                                    !law.IsDeleted &&
                                    law.Category.ToUpper() != Constants.CGI.ToUpper() &&
                                    law.Category.ToUpper() != Constants.Douane.ToUpper())
                      .ToListAsync();
            var laws = laws_.Where(law => law.Statu.Order != 6).ToList();
            foreach (var l in laws)
            {
                Guid PhaseLawId = l.PhaseLawIds.FirstOrDefault(l => l.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Empty;
                var lawVm = new LawVm();
                lawVm.Id = l.Id;
                lawVm.Label = l.Label;
                lawVm.Number = l.Number;
                lawVm.Year = l.Year;
                lawVm.PhaseLawId = PhaseLawId;
                lawVm.PhaseIds = l.PhaseLawIds.Where(p => p.Statu == PhaseStatu.OPENED.ToString()).Select(p => p.PhaseId.ToString()!.ToLower()).ToList();
                lawVm.CommissionName = l.Commission?.Name!;
                lawVm.PhaseName = await GetPhaseName(l);
                lawVm.TeamName = l.Team?.Name;
                lawVm.HasAction = l.ProgrammedDateCommRead1 != null;
                lawVm.StatuName = l.Statu?.Label!;
                if (listLaws.Count(lw => lw.Id == l.Id) == 0)
                {
                    listLaws.Add(lawVm);
                }
            }
            return listLaws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(SetLawInfoAsync)}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<LawInfo> GetLawInfoAsync(Guid LawId)
    {
        try
        {
            DateTime DefaultDate = Convert.ToDateTime("01/01/0001 00:00:00");
            var law = await findAsync(l => l.Id == LawId);
            if (law is null)
                return null!;
            LawInfo lawInfo = new();

            lawInfo.LawId = law.Id;
            lawInfo.LegId = law.IdLegislative == null ? Guid.Empty : (Guid)law.IdLegislative!;
            lawInfo.YearId = law.LegislativeYearId == null ? Guid.Empty : (Guid)law.LegislativeYearId!;
            lawInfo.SessionId = law.IdSession is null ? Guid.Empty : (Guid)law.IdSession!;
            lawInfo.LawLabel = law.Label;
            lawInfo.IdTeam = law.IdEquipe == null ? Guid.Empty : (Guid)law.IdEquipe!;
            lawInfo.DateAffectationBureau = law.DateAffectationBureau != DefaultDate ? law.DateAffectationBureau : null;
            lawInfo.CommissionId = law.IdCommission == null ? Guid.Empty : (Guid)law.IdCommission!;
            lawInfo.DateAffectationCommission1 = law.DateAffectationCommission1 != DefaultDate ? law.DateAffectationCommission1! : null;
            lawInfo.ProgrammedDateCommRead1 = law.ProgrammedDateCommRead1 != DefaultDate ? law.ProgrammedDateCommRead1 : null;
            lawInfo.DateFinAmendments1 = law.DateFinAmendments1 != DefaultDate ? law.DateFinAmendments1 : null;
            lawInfo.DateVoteCommRead1 = law.DateVoteCommRead1 != DefaultDate ? law.DateVoteCommRead1 : null;
            lawInfo.DateAffectationSeance1 = law.DateAffectationSeance1 != DefaultDate ? law.DateAffectationSeance1 : null;
            lawInfo.DateVoteSeanceRead1 = law.DateVoteSeanceRead1 != DefaultDate ? law.DateVoteSeanceRead1 : null;
            lawInfo.DateAffectationCdc = law.DateAffectationCdc != DefaultDate ? law.DateAffectationCdc : null;
            lawInfo.DateAffectationBureau2 = law.DateAffectationBureau2 != DefaultDate ? law.DateAffectationBureau2 : null;
            lawInfo.DateAffectationCommission2 = law.DateAffectationCommission2 != DefaultDate ? law.DateAffectationCommission2 : null;
            lawInfo.ProgrammedDateCommRead2 = law.ProgrammedDateCommRead2 != DefaultDate ? law.ProgrammedDateCommRead2 : null;
            lawInfo.DateFinAmendments2 = law.DateFinAmendments2 != DefaultDate ? law.DateFinAmendments2 : null;
            lawInfo.DateVoteCommRead2 = law.DateVoteCommRead2;
            lawInfo.DateAffectationSeance2 = law.DateAffectationSeance2 != DefaultDate ? law.DateAffectationSeance2 : null;
            lawInfo.DateVoteSeanceRead2 = law.DateVoteSeanceRead2 != DefaultDate ? law.DateVoteSeanceRead2 : null;
            lawInfo.PublishedDate = law.DatePublication != DefaultDate ? law.DatePublication : null;
            lawInfo.Number = law.PublishNumber;


            return lawInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {GetLawInfoAsync}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<ServerResponse> SetPhaseLawAsync(Guid LawId, Guid PhaseLawId, Guid LastModifiedBy)
    {
        try
        {
            var law = await findAsync(l => l.Id == LawId);
            if (law is null)
                return new ServerResponse(false, $"Law Where Id : {LawId} Is Not Found");
            //law.PhaseLawId = PhaseLawId;
            //law.LastModifiedBy = LastModifiedBy;
            //law.ModifictationDate = DateTime.UtcNow;
            //await UpdateAsync(law);
            _db.PhaseLawIds.Add(new DataAccess.Models.PhaseLawId { LawId = law.Id, PhaseId = PhaseLawId, Statu = PhaseStatu.OPENED.ToString() });
            _db.SaveChanges();
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {SetPhaseLawAsync}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetAllLawsCommitionAsync(List<Ids> ids, Guid IdCommission)
    {
        try
        {
            List<LawVm> laws = new();
            foreach (var id in ids)
            {
                Guid phaseId = Guid.Parse(id.Id);
                var laws_ = await _dbSet
                    .AsNoTracking()
                    .Include(t => t.Team)
                    .Include(l => l.Commission)
                    .Include(l => l.PhaseLaw)
                    .Include(l => l.Statu)
                    .Where(l => l.PhaseLawId == phaseId && l.IsDeleted == false && l.IdCommission == IdCommission)
                    .OrderByDescending(l => l.CreationDate)
                    .Select(l => new LawVm
                    {
                        Id = l.Id,
                        Label = l.Label,
                        Number = l.Number,
                        Year = l.Year,
                        PhaseLawId = l.PhaseLawId,
                        CommissionName = l.Commission!.Name,
                        PhaseName = l.PhaseLaw!.Title,
                        TeamName = l.Team!.Name,
                        HasAction = l.ProgrammedDateCommRead1 == null ? false : true,
                        StatuName = l.Statu!.Label ?? string.Empty
                    })
                   .ToListAsync();
                laws.AddRange(laws_);
            }
            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetCGILawAsync function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetLawsByIdEnititesAsync(List<Guid> phasesId, List<Guid>? EntitiesId, Guid UserId)
    {
        try
        {
            List<Law> laws = new();
            List<Law> laws_ = new();
            var user = await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == UserId);
            foreach (Guid phaseId in phasesId)
            {
                var commLaws = await _dbSet.
                     AsNoTracking().
                     AsSplitQuery()
                    .Include(t => t.Team)
                    .Include(t => t.Statu)
                    .Include(l => l.Commission)
                    .Include(l => l.PhaseLawIds)
                    .Where(l => l.PhaseLawIds.Count(p => p.PhaseId == phaseId && p.Statu == PhaseStatu.OPENED.ToString()) > 0).ToListAsync();
                if (commLaws.Count > 0)
                    laws.AddRange(commLaws);
            }

            if (EntitiesId is null)
            {
                laws_ = laws;
            }
            else
            {
                if (user!.Roles.Any(r => r.Name == RoleEnum.DIRECTEUR_LEGISLATION.ToString()))
                {
                    laws_ = laws;
                }
                else
                {
                    foreach (var entity in EntitiesId!)
                    {
                        var _laws = laws.Where(l => l.IdCommission == entity).ToList();
                        if (_laws.Count > 0)
                            laws_.AddRange(_laws);
                    }
                }
            }

            List<LawVm> listLaws = new();
            foreach (var l in laws_)
            {
                Guid PhaseLawId = l.PhaseLawIds.FirstOrDefault(l => l.Statu == PhaseStatu.OPENED.ToString())?.PhaseId ?? Guid.Empty;
                var lawVm = new LawVm();
                lawVm.Id = l.Id;
                lawVm.Label = l.Label;
                lawVm.Number = l.Number;
                lawVm.Year = l.Year;
                lawVm.PhaseLawId = PhaseLawId;
                lawVm.PhaseIds = l.PhaseLawIds.Where(p => p.Statu == PhaseStatu.OPENED.ToString()).Select(p => p.PhaseId.ToString()!.ToLower()).ToList();
                lawVm.CommissionName = l.Commission?.Name!;
                lawVm.PhaseName = await GetPhaseName(l);
                lawVm.TeamName = l.Team!.Name;
                lawVm.HasAction = l.ProgrammedDateCommRead1 != null;
                lawVm.StatuName = l.Statu?.Label!;
                if (listLaws.Count(lw => lw.Id == l.Id) == 0)
                {
                    listLaws.Add(lawVm);
                }
            }
            return listLaws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetLawsByIdEnititesAsync function errors", nameof(LawRepository));
            throw;
        }
    }
    private async Task<string> GetPhaseName(Law law)
    {
        string phaseName = string.Empty;
        List<Guid> IDS = new List<Guid>();
        if (law.PhaseLawIds.Count(p => p.Statu == PhaseStatu.OPENED.ToString()) > 0)
        {
            foreach (var phase in law.PhaseLawIds.Where(p => p.Statu == PhaseStatu.OPENED.ToString()))
            {
                var ph = await _db.Phases.FirstOrDefaultAsync(p => p.Id == phase.PhaseId);
                if (!IDS.Any(id => id == ph.Id))
                {
                    phaseName += ph?.Title + " - ";
                }
                IDS.Add(ph.Id);
            }
        }
        else
        {
            List<Phase> phases = new List<Phase>();
            foreach (var phase in law.PhaseLawIds.ToList())
            {
                var ph = await _db.Phases.FirstOrDefaultAsync(p => p.Id == phase.PhaseId);
                phases.Add(ph);
            }
            phaseName = phases.FirstOrDefault(p => p.Order == phases.Max(m => m.Order))!.Title;
        }

        return phaseName.Remove(phaseName.Length - 1);
    }
    public async Task<ServerResponse> DeleteLawAsync(Guid LawId, Guid LastModifiedBy)
    {
        try
        {
            var law = await findAsync(l => l.Id == LawId);
            if (law is null)
                return new ServerResponse(false, $"Law Where Id {LawId} Not Found");
            law.IsDeleted = true;
            law.LastModifiedBy = LastModifiedBy;
            law.ModifictationDate = DateTime.UtcNow;
            await UpdateAsync(law);
            var nodes = _db.Nodes.Where(n => n.LawId == LawId).ToList();
            foreach (var node in nodes.ToList())
            {
                node.IsDeleted = true;
                node.LastModifiedBy = LastModifiedBy;
                node.ModifictationDate = DateTime.UtcNow;
                _db.Entry(node).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(DeleteLawAsync)}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<ServerResponse> SetLawStatuAsync(LawStatuVm statuVm)
    {
        try
        {
            var law = await findAsync(l => l.Id == statuVm.LawId);
            if (law is null)
                return new ServerResponse(false, $"Law Where Id : {statuVm.LawId} Is Not Found");
            var statu = await _db.Statuts.FirstOrDefaultAsync(t => t.Id == statuVm.StatuLaw);
            //قيد الدرس - القراءة 1 – أحيل على مكتب المجلس
            if (statu?.Order == (int)LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_OFFICE_COUNCIL)
                law.DateAffectationBureau = (DateTime)statuVm.DateSetStatu!;
            //قيد الدرس - القراءة 1 – أحيل على اللجنة
            if (statu?.Order == (int)LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_COMMISSION)
                law.DateAffectationCommission1 = (DateTime)statuVm.DateSetStatu!;
            //قيد الدرس - القراءة 1 – برمجته اللجنة
            if (statu?.Order == (int)LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_PROGRAMMED_IT_COMMISSION)
                law.ProgrammedDateCommRead1 = statuVm.DateSetStatu;
            //قيد الدرس - القراءة 1 – صوتت عليه اللجنة
            if (statu?.Order == (int)LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_VOTED_COMMISSION)
                law.DateVoteCommRead1 = (DateTime)statuVm.DateSetStatu!;
            //صادق عليه مجلس النواب - القراءة 1
            if (statu?.Order == (int)LawStatu.APPROVED_IT_HOUSE_OF_REPRESENTATIVES_READING_ONE)
                law.DateVoteSeanceRead1 = statuVm.DateSetStatu;
            //قيد الدرس - القراءة 2 – أحيل على مكتب المجلس
            if (statu?.Order == (int)LawStatu.RESTRICTION_LESSON_REDING_TWO_REFER_OFFICE_COUNCIL)
                law.DateAffectationBureau2 = statuVm.DateSetStatu;
            //قيد الدرس - القراءة 2 – أحيل على اللجنة
            if (statu?.Order == (int)LawStatu.RESTRICTION_LESSON_REDING_TWO_REFER_COMMISSION)
                law.DateAffectationCommission2 = (DateTime)statuVm.DateSetStatu!;
            //قيد الدرس - القراءة 2 – برمجته اللجنة
            if (statu?.Order == (int)LawStatu.RESTRICTION_LESSON_REDING_TWO_REFER_PROGRAMMED_IT_COMMISSION)
                law.ProgrammedDateCommRead2 = statuVm.DateSetStatu;
            //قيد الدرس - القراءة 2 –  صوتت عليه اللجنة
            if (statu?.Order == (int)LawStatu.RESTRICTION_LESSON_REDING_TWO_REFER_VOTED_COMMISSION)
                law.DateVoteCommRead2 = statuVm.DateSetStatu;
            //صادق عليه مجلس النواب - القراءة 2
            if (statu?.Order == (int)LawStatu.APPROVED_IT_HOUSE_OF_REPRESENTATIVES_READING_TWO)
                law.DateVoteSeanceRead2 = statuVm.DateSetStatu;
            law.StatuId = statuVm.StatuLaw;
            law.LastModifiedBy = statuVm.LastModifiedBy;
            law.ModifictationDate = DateTime.UtcNow;
            await UpdateAsync(law);
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {SetLawStatuAsync}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<ServerResponse> GetLawStatuAsync(Guid LawId)
    {
        try
        {
            var law = await _dbSet.Include(l => l.Statu).FirstOrDefaultAsync(l => l.Id == LawId);
            if (law is null)
                return new ServerResponse(false, $"Law Where Id : {LawId} Is Not Found");
            if (law.Statu is null)
                return new ServerResponse(true, "0");
            return new ServerResponse(true, law.Statu.Order.ToString()!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {GetLawStatuAsync}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawDetail>> GetLawsByTeamId(Guid teamId)
    {
        try
        {
            var laws = await _dbSet
                .Include(l => l.Team)
                .Include(l => l.Commission)
                .Include(l => l.PhaseLaw)
                .AsNoTracking()
                .AsSingleQuery()
                .Where(law => law.IdEquipe == teamId).ToListAsync();

            var laws_ = laws.Select(l => new LawDetail()
            {
                Id = l.Id,
                Label = l.Label,
                Year = l.Year,
                Number = l.Number,
                PhaseName = l.PhaseLaw?.Title!,
                TeameName = l.Team?.Name!,
                CommissionName = l.Commission?.Name!,
                DateAffectaionBureau_1 = l.DateAffectationBureau,
                DateAffectaionCommission_1 = l.DateAffectationCommission1,
                DateProgramationCommission_1 = l.ProgrammedDateCommRead1,
                DateVoteCommission_1 = l.DateVoteCommRead1,
                DateVoteSession_1 = l.DateVoteSeanceRead1,
                DateAffectaionBureau_2 = l.DateAffectationBureau2,
                DateAffectaionCommission_2 = l.DateAffectationCommission2,
                DateProgramationCommission_2 = l.ProgrammedDateCommRead2,
                DateVoteCommission_2 = l.DateVoteCommRead2,
                DateVoteSession_2 = l.DateVoteSeanceRead2,
            }).ToList();
            return laws_;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetLawsByTeamId function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawDetail>> GetLawByIds(List<Guid> Ids)
    {
        try
        {
            var laws = await _dbSet
                .Include(l => l.Team)
                .Include(l => l.Commission)
                .Include(l => l.PhaseLaw)
                .AsNoTracking()
                .AsSingleQuery()
                .Where(law => Ids.Any(i => i == law.Id)).
                ToListAsync();

            var laws_ = laws.Select(l => new LawDetail()
            {
                Id = l.Id,
                Label = l.Label,
                Year = l.Year,
                Number = l.Number,
                PhaseName = l.PhaseLaw?.Title!,
                TeameName = l.Team?.Name!,
                CommissionName = l.Commission?.Name!,
                DateAffectaionBureau_1 = l.DateAffectationBureau,
                DateAffectaionCommission_1 = l.DateAffectationCommission1,
                DateProgramationCommission_1 = l.ProgrammedDateCommRead1,
                DateVoteCommission_1 = l.DateVoteCommRead1,
                DateVoteSession_1 = l.DateVoteSeanceRead1,
                DateAffectaionBureau_2 = l.DateAffectationBureau2,
                DateAffectaionCommission_2 = l.DateAffectationCommission2,
                DateProgramationCommission_2 = l.ProgrammedDateCommRead2,
                DateVoteCommission_2 = l.DateVoteCommRead2,
                DateVoteSession_2 = l.DateVoteSeanceRead2,
            }).ToList();
            return laws_;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {GetLawByIds}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetLawsToPrint(Guid ComId)
    {
        try
        {
            var laws = await _dbSet
                .AsNoTracking()
                .AsSingleQuery()
                 .Include(law => law.Statu)
                .Where(law => law.IdCommission == ComId && law.ProgrammedDateCommRead1 != null)
                .ToListAsync();

            var laws_ = laws.Select(l => new LawVm()
            {
                Id = l.Id,
                StatuName = l.Statu?.Label ?? "",
                Label = l.Label
            }).ToList();
            return laws_;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetLawsToPrint function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetLawsForReadTwo(Guid StatuId)
    {
        try
        {
            var laws = await _dbSet.AsNoTracking()
                               .AsSplitQuery()
                               .Include(l => l.PhaseLaw)
                               .Include(l => l.Team)
                               .Where(l => l.StatuId == StatuId && !l.IsDeleted)
                               .Select(l => new LawVm { Id = l.Id, Label = l.Label, PhaseLawId = l.PhaseLawId, PhaseName = l.PhaseLaw!.Title, Year = l.Year, Number = l.Number, TeamName = l.Team!.Name, CommissionName = l.Commission!.Name })
                               .ToListAsync();

            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} {GetLawsForReadTwo} Error", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetAllLawsForAll(List<Guid> PhaseIds)
    {
        try
        {
            var laws = await _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .Include(t => t.Team)
                .Include(l => l.Commission)
                .Include(l => l.Statu)
                .Include(l => l.PhaseLawIds)
                .Where(l => l.IsDeleted == false &&
                            l.Category.ToUpper() != Constants.CGI.ToUpper() &&
                            l.Category.ToUpper() != Constants.Douane.ToUpper() &&
                            l.Statu!.Order >= 1)
                .OrderByDescending(l => l.CreationDate)
                .Select(l => new LawVm
                {
                    Id = l.Id,
                    Label = l.Label,
                    Number = l.Number,
                    Year = l.Year,
                    CommissionName = l.Commission!.Name,
                    StatuName = l.Statu!.Label ?? string.Empty,
                    TeamName = l.Team!.Name
                }).ToListAsync();

            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetAllLawsForAll function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<ServerResponse> setDateIhalaLaw(LawDate lawDate)
    {
        try
        {
            var law = await findAsync(l => l.Id == lawDate.LawId);
            if (law is null)
                return new ServerResponse(false, $"Law Where Id : {lawDate.LawId} Is Not Found");
            law.LastModifiedBy = lawDate.LastModifiedBy;
            law.ModifictationDate = DateTime.UtcNow;
            law.DateAffectationBureau2 = lawDate.DateModification;
            await UpdateAsync(law);
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {setDateIhalaLaw}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<List<LawVm>> GetAllPreparationLawsAsync(Guid PhaseId, Guid TeamId)
    {
        try
        {
            List<LawVm> allLaws = new List<LawVm>();
            var laws = await _dbSet
                         .AsNoTracking()
                         .AsSplitQuery()
                         .Include(p => p.PhaseLawIds)
                         .Where(l => l.PhaseLawId != PhaseId &&
                                     l.IsDeleted == false &&
                                     l.Category.ToUpper() != Constants.CGI.ToUpper() &&
                                     l.Category.ToUpper() != Constants.Douane.ToUpper())
                         .OrderByDescending(l => l.CreationDate)
                         .Select(l =>
                         new LawVm
                         {
                             Id = l.Id,
                             Label = l.Label,
                             PhaseLawId = l.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())!.PhaseId,
                             PhaseName = l.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())!.Phases!.Title,
                             CreationDate = l.CreationDate,
                             SubmitedDate = l.DateAffectationBureau,
                             PhaseIds = l.PhaseLawIds.Where(p => p.Statu == PhaseStatu.OPENED.ToString()).Select(p => p.PhaseId.ToString()).ToList(),
                         })
                         .ToListAsync();
            allLaws.AddRange(laws);

            var laws_ = await _dbSet
                        .AsNoTracking()
                        .AsSplitQuery()
                        .Include(p => p.PhaseLawIds)
                        .Where(l => l.IdEquipe == TeamId && l.IsDeleted == false && l.Category != "CGI")
                        .OrderByDescending(l => l.CreationDate)
                        .Select(l =>
                        new LawVm
                        {
                            Id = l.Id,
                            Label = l.Label,
                            PhaseLawId = l.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())!.PhaseId,
                            PhaseName = l.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())!.Phases!.Title,
                            CreationDate = l.CreationDate,
                            SubmitedDate = l.DateAffectationBureau,
                            PhaseIds = l.PhaseLawIds.Where(p => p.Statu == PhaseStatu.OPENED.ToString()).Select(p => p.PhaseId.ToString()).ToList(),
                        })
                        .ToListAsync();

            foreach (var law in laws_)
            {
                if (allLaws.Count(l => l.Id == law.Id) == 0)
                {
                    allLaws.Add(law);
                }
            }

            return allLaws.OrderByDescending(l => l.CreationDate).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}  Error on {GetAllPreparationLawsAsync}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<ServerResponse> CheckLawExiste(string Number, int Year)
    {
        try
        {
            var law = await findAsync(l => l.Number == Number && l.Year == Year);
            if (law is null)
                return new ServerResponse(false, $"Law where Year:{Year} and Number:{Number} not existe");
            return new ServerResponse(true, $"Law where Year:{Year} and Number:{Number} already exists");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message}  Error on {CheckLawExiste}", nameof(LawRepository));
            throw;
        }
    }
    public async Task<ServerResponse> CreateLawAsync(LawDto lawDto)
    {
        try
        {
            var phase = await _db.Phases.FirstAsync(p => p.IsDeleted == false && p.Order == 0);
            var statu = await _db.Statuts.AsNoTracking().FirstOrDefaultAsync(s => s.Order == 0);
            Law law = new();
            law.Label = lawDto.Label;
            law.Year = lawDto.Year;
            law.Number = lawDto.Number;
            law.Category = lawDto.Category!;
            law.Type = lawDto.Type;
            law.IsDeleted = false;
            law.NodeHierarchyFamillyId = Guid.Parse(lawDto.NodeHierarchyFamillyId);
            law.CreatedBy = Guid.Parse("F37BE3F3-9E2E-4E64-90EF-DCF48F38BF48");
            law.CreationDate = DateTime.UtcNow;
            // law.IdEquipe = Guid.Parse("6F271E27-B918-459E-67C5-08DBD4755AEF");
            // law.PhaseLawId = phase.Id;
            law.StatuId = statu?.Id;
            law.IdFinance = lawDto.Id;
            var result = await CreateAsync(law);
            await _db.PhaseLawIds.AddAsync(new PhaseLawId()
            {
                LawId = law.Id,
                PhaseId = phase.Id,
                Statu = PhaseStatu.OPENED.ToString()
            });
            await _db.SaveChangesAsync();
            return new ServerResponse(result, result ? "success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} {nameof(AddLawAsync)} function errors", nameof(LawRepository));
            throw;
        }
    }
    public async Task<Law> getLawByYearAsync(int year)
    {
        try
        {
            var law = await _dbSet
                .AsNoTracking()
                .AsSplitQuery()
                .Include(l => l.PhaseLawIds)
                .ThenInclude(l => l.Phases)
                .FirstOrDefaultAsync(l => l.Year == year);
            return law;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} getLawByYearAsync function errors", nameof(LawRepository));
            throw;
        }
    }

    public async Task<List<LawListDto>> GetAllLawsWithPhases()
    {
        try
        {
            List<LawListDto> laws = new List<LawListDto>();
            var records = await _dbSet.Where(law =>
                                    law.Category.ToUpper() != Constants.CGI.ToUpper() &&
                                    law.Category.ToUpper() != Constants.Douane.ToUpper())
                .OrderBy(law => law.Year).ToListAsync();
            foreach (var record in records)
            {
                LawListDto law = new LawListDto();
                law.Id = record.Id;
                law.Label = record.Label;
                var phaseLawIds = await _db.PhaseLawIds.Include(ph => ph.Phases).Where(p => p.LawId == record.Id).ToListAsync();
                law.Phases = phaseLawIds.Select(p => new PhaseDto() { Id = p.PhaseId ?? Guid.Empty, Label = p.Phases.Title, Order = p.Phases.Order }).ToList();
                laws.Add(law);
            }
            return laws;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetAllLawsWithPhases function errors", nameof(LawRepository));
            throw;
        }
    }

    public async Task<List<LawVm>> getAllReferences()
    {
        var laws = await _dbSet.Where(law =>
                                   law.Category.ToUpper() == Constants.CGI.ToUpper() ||
                                   law.Category.ToUpper() == Constants.Douane.ToUpper())
               .OrderBy(law => law.Year)
               .Select(law => new LawVm
               {
                   Id = law.Id,
                   Year = law.Year,
                   Label = law.Label,
               })
               .ToListAsync();
        return laws;
    }
}

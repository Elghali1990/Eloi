namespace E.Loi.Services.Repositories;

public class StatisticsRepository(ILogger _logger, LawDbContext _db) : IStatisticsRepository
{
    public async Task<List<StatisticsVM>> StatisticsByCommittees()
    {
        try
        {
            List<StatisticsVM> statistics = new();
            var commissions = await _db.Teams.Where(com => com.TeamType == nameof(TeamTypes.COMISSION) && !com.IsDeleted).OrderBy(c => c.Ordre).ToListAsync();
            foreach (var commission in commissions)
            {
                int textLawsNumber = await _db.Laws.CountAsync(law => law.Category.Trim() == "Text Loi" && law.IdCommission == commission.Id && !law.IsDeleted);
                int projetLawsNumber = await _db.Laws.CountAsync(law => law.Category.Trim() == "Projet Loi" && law.IdCommission == commission.Id && !law.IsDeleted);
                statistics.Add(new StatisticsVM { Id = commission.Id, Label = commission.Name, TextLaws_Number = textLawsNumber, LawProjects_Number = projetLawsNumber });
            }
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On ${StatisticsByCommittees}", nameof(StatisticsRepository));
            throw;
        }
    }
    public async Task<List<StatisticsVM>> StatisticsByParliamentaryTeams()
    {
        try
        {
            List<StatisticsVM> statistics = new();
            var teams = await _db.Teams.Where(t => t.TeamType == nameof(TeamTypes.PARTIES) && !t.IsDeleted).ToListAsync();
            foreach (var team in teams)
            {
                int textLawsNumber = await _db.Laws.CountAsync(law => law.Category.Trim() == "Text Loi" && law.IdEquipe == team.Id && !law.IsDeleted);
                int projetLawsNumber = await _db.Laws.CountAsync(law => law.Category.Trim() == "Projet Loi" && law.IdEquipe == team.Id && !law.IsDeleted);
                statistics.Add(new StatisticsVM { Id = team.Id, Label = team.Name, TextLaws_Number = textLawsNumber, LawProjects_Number = projetLawsNumber });
            }
            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On ${StatisticsByParliamentaryTeams}", nameof(StatisticsRepository));
            throw;
        }
    }
    public async Task<List<StatisticsDtos>> StatisticsReadOne()
    {
        try
        {
            var status = await _db.Statuts.Where(statu => statu.Order > 0 && statu.Order <= 5).OrderBy(s => s.Order).ToListAsync();
            List<StatisticsDtos> statisticsDtos = new List<StatisticsDtos>();
            foreach (var statu in status)
            {
                int counter = await _db.Laws.CountAsync(l => l.StatuId == statu.Id && l.ProgrammedDateCommRead1 != null);
                var IDS = _db.Laws.Where(l => l.StatuId == statu.Id && l.ProgrammedDateCommRead1 != null).Select(l => l.Id).ToList();
                statisticsDtos.Add(new StatisticsDtos { StatuLabel = statu.Label!, Counter = counter, Ids = IDS });
            }
            return statisticsDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On StatisticsReadOne", nameof(StatisticsRepository));
            throw;
        }
    }

    public async Task<List<StatisticsDtos>> StatisticsReadOne(SearchDtos search)
    {
        try
        {
            var status = await _db.Statuts.Where(statu => statu.Order > 0 && statu.Order <= 5).OrderBy(s => s.Order).ToListAsync();
            List<StatisticsDtos> statisticsDtos = new List<StatisticsDtos>();
            foreach (var statu in status)
            {
                var laws = await LawsCounter(statu.Id, search, statu);
                statisticsDtos.Add(new StatisticsDtos { StatuLabel = statu.Label!, Counter = laws.Count(), Ids = laws.Select(l => l.Id).ToList() });
            }
            return statisticsDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On StatisticsReadOne", nameof(StatisticsRepository));
            throw;
        }
    }

    public async Task<List<StatisticsDtos>> StatisticsReadTwo()
    {
        try
        {
            var status = await _db.Statuts.Where(statu => statu.Order >= 6 && statu.Order <= 10).OrderBy(s => s.Order).ToListAsync();
            List<StatisticsDtos> statisticsDtos = new List<StatisticsDtos>();
            foreach (var statu in status)
            {
                int counter = await _db.Laws.CountAsync(l => l.StatuId == statu.Id && l.ProgrammedDateCommRead1 != null);
                var IDS = _db.Laws.Where(l => l.StatuId == statu.Id && l.ProgrammedDateCommRead1 != null).Select(l => l.Id).ToList();
                statisticsDtos.Add(new StatisticsDtos { StatuLabel = statu.Label!, Counter = counter, Ids = IDS });
            }
            return statisticsDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On StatisticsReadTwo", nameof(StatisticsRepository));
            throw;
        }
    }

    public async Task<List<StatisticsDtos>> StatisticsReadTwo(SearchDtos search)
    {
        var status = await _db.Statuts.Where(statu => statu.Order >= 6 && statu.Order <= 10).OrderBy(s => s.Order).ToListAsync();
        List<StatisticsDtos> statisticsDtos = new List<StatisticsDtos>();
        foreach (var statu in status)
        {
            var laws = await LawsCounter(statu.Id, search, statu);
            statisticsDtos.Add(new StatisticsDtos { StatuLabel = statu.Label!, Counter = laws.Count(), Ids = laws.Select(l => l.Id).ToList() });
        }
        return statisticsDtos;
    }

    public async Task<List<Law>> LawsCounter(Guid IdStatu, SearchDtos search, Statut statu)
    {
        List<Law> laws = new();
        bool startDateSupplide = search.StartDate.HasValue;
        bool endDateSupplide = search.EndDate.HasValue;
        bool IdCommissionSupplide = !(search.IdCommission == null || search.IdCommission == Guid.Empty);

        if (GetIntFromEnumName(LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_OFFICE_COUNCIL.ToString()) == statu.Order)
        {
            laws = await _db.Laws
                                .AsNoTracking()
                                .AsSingleQuery()
                                .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                                 && (!startDateSupplide || l.DateAffectationBureau >= search!.StartDate)
                                 && (!endDateSupplide || l.DateAffectationBureau <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_COMMISSION.ToString()))
        {
            laws = await _db.Laws
                              .AsNoTracking()
                              .AsSingleQuery()
                              .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                               && (!startDateSupplide || l.DateAffectationCommission1 >= search!.StartDate)
                               && (!endDateSupplide || l.DateAffectationCommission1 <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_PROGRAMMED_IT_COMMISSION.ToString()))
        {

            laws = await _db.Laws
                     .AsNoTracking()
                     .AsSingleQuery()
                     .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                      && (!startDateSupplide || l.ProgrammedDateCommRead1 >= search!.StartDate)
                      && (!endDateSupplide || l.ProgrammedDateCommRead1 <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.RESTRICTION_LESSON_REDING_ONE_REFER_VOTED_COMMISSION.ToString()))
        {
            laws = await _db.Laws
                   .AsNoTracking()
                   .AsSingleQuery()
                   .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                    && (!startDateSupplide || l.DateVoteCommRead1 >= search!.StartDate)
                    && (!endDateSupplide || l.DateVoteCommRead1 <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.APPROVED_IT_HOUSE_OF_REPRESENTATIVES_READING_ONE.ToString()))
        {
            laws = await _db.Laws
               .AsNoTracking()
               .AsSingleQuery()
               .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                && (!startDateSupplide || l.DateVoteSeanceRead1 >= search!.StartDate)
                && (!endDateSupplide || l.DateVoteSeanceRead1 <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.RESTRICTION_LESSON_REDING_TWO_REFER_OFFICE_COUNCIL.ToString()))
        {
            laws = await _db.Laws
                              .AsNoTracking()
                              .AsSingleQuery()
                              .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                               && (!startDateSupplide || l.DateAffectationBureau2 >= search!.StartDate)
                               && (!endDateSupplide || l.DateAffectationBureau2 <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.RESTRICTION_LESSON_REDING_TWO_REFER_COMMISSION.ToString()))
        {
            laws = await _db.Laws
                     .AsNoTracking()
                     .AsSingleQuery()
                     .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                      && (!startDateSupplide || l.DateAffectationCommission2 >= search!.StartDate)
                      && (!endDateSupplide || l.DateAffectationCommission2 <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.RESTRICTION_LESSON_REDING_TWO_REFER_PROGRAMMED_IT_COMMISSION.ToString()))
        {
            laws = await _db.Laws
                                 .AsNoTracking()
                                 .AsSingleQuery()
                                 .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                                  && (!startDateSupplide || l.ProgrammedDateCommRead2 >= search!.StartDate)
                                  && (!endDateSupplide || l.ProgrammedDateCommRead2 <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.RESTRICTION_LESSON_REDING_TWO_REFER_VOTED_COMMISSION.ToString()))
        {
            laws = await _db.Laws
                         .AsNoTracking()
                         .AsSingleQuery()
                         .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                          && (!startDateSupplide || l.DateVoteCommRead2 >= search!.StartDate)
                          && (!endDateSupplide || l.DateVoteCommRead2 <= search.EndDate)).ToListAsync();
        }
        if (statu.Order == GetIntFromEnumName(LawStatu.APPROVED_IT_HOUSE_OF_REPRESENTATIVES_READING_TWO.ToString()))
        {
            laws = await _db.Laws
                     .AsNoTracking()
                     .AsSingleQuery()
                     .Where(l => l.StatuId == IdStatu && (!IdCommissionSupplide || l.IdCommission == search!.IdCommission)
                      && (!startDateSupplide || l.DateVoteSeanceRead2 >= search!.StartDate)
                      && (!endDateSupplide || l.DateVoteSeanceRead2 <= search.EndDate)).ToListAsync();
        }
        return laws;
    }

    private int GetIntFromEnumName(string name)
    {
        return (int)Enum.Parse(typeof(LawStatu), name);
    }
}

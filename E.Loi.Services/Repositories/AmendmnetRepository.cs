namespace E.Loi.Services.Repositories;

public class AmendmnetRepository : BaseRepository<Amendment>, IAmendmnetRepository
{
    public AmendmnetRepository(ILogger logger, LawDbContext db) : base(logger, db) { }
    public async Task<ServerResponse> CreateAmendmantAsync(AmendmentVm amendmentVm)
    {
        try
        {
            Amendment amendment = new()
            {
                NodeId = amendmentVm.NodeId,
                TeamId = amendmentVm.TeamId,
                //Number = amendmentVm.Number,
                Supressed = false,
                AmendmentType = amendmentVm.AmendmentType,
                AmendmentIntent = amendmentVm.AmendmentIntent,
                AmendmentsStatus = AmendmentsStatus.EDITABLE.ToString(),
                Title = amendmentVm.Title,
                Content = amendmentVm.Content,
                Justification = amendmentVm.Justification,
                OriginalContent = amendmentVm.OriginalContent,
                ArticleRef = string.Empty,
                CreatedBy = amendmentVm.CreatedBy,
                IsAmendementRattrape = amendmentVm.IsAmendementRattrape,
                IsAmendementSeance = amendmentVm.IsAmendementSeance,
                CreationDate = DateTime.UtcNow,
                OrderParagraphe = amendmentVm.ParagrapheOrder,
                TitreParagraphe = amendmentVm.ParagreapheTitle
            };
            await _dbSet.AddAsync(amendment);
            if (await _db.SaveChangesAsync() > 0)
            {
                if (amendmentVm.AmendmentIds != null)
                {
                    foreach (var Id in amendmentVm.AmendmentIds)
                    {
                        amendment.AmendmentClusters.Add(await GetByIdAsync(Id));
                        await _db.SaveChangesAsync();
                    }
                }
                return new ServerResponse(true, "Saved");
            }
            return new ServerResponse(false, "Error on save amendment");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(CreateAmendmantAsync), nameof(AmendmnetRepository));
            // _logger.LogError(ex, $"{ex.Message} CreateAmendmantAsync function errors", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> DeleteAmendmantAsync(Guid amendmentId, Guid UserId)
    {
        try
        {
            var amendment = await _dbSet.Include(a => a.AmendmentClusters).Include(a => a.Node).FirstOrDefaultAsync(a => a.Id == amendmentId);
            if (amendment == null) return new ServerResponse(false, $"Amendment where Id :{amendmentId}");
            if (amendment.AmendmentClusters != null)
            {
                foreach (var amd in amendment.AmendmentClusters.ToList())
                {
                    amendment.AmendmentClusters.Remove(amd);
                    await _db.SaveChangesAsync();
                }
            }
            _dbSet.Remove(amendment);
            await _db.SaveChangesAsync();
            if (amendment.AmendmentIntent == AmendmentIntents.ADDITION.ToString())
            {
                var nodeAmendments = await _dbSet.CountAsync(am => am.NodeId == amendment.NodeId);
                if (nodeAmendments == 0)
                {
                    if (amendment.CreatedFromId == null)
                    {
                        _db.Nodes.Remove(amendment.Node);
                        await _db.SaveChangesAsync();
                        await RecalculateOrderOnDeletetNode(amendment.Node);
                    }
                }
            }
            return new ServerResponse(true, "Deleted");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(UpdateAmendmantAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "UpdateAmendmantAsync function error", nameof(AmendmnetRepository));
            return new ServerResponse(false, "error on Delete amendment");
        }
    }
    public async Task<AmendmentVm> GetAmendmentByIdAsync(Guid Id)
    {
        try
        {
            var amendment = await _db.Amendments.Include(am => am.Team).FirstOrDefaultAsync(am => am.Id == Id);
            if (amendment is null)
                return null!;
            var amdVm = new AmendmentVm
            {
                NodeId = amendment.NodeId,
                TeamId = amendment.TeamId,
                TeamName = amendment.Team.Name,
                Number = amendment.Number,
                AmendmentIntent = amendment.AmendmentIntent,
                AmendmentType = amendment.AmendmentType,
                Title = amendment.Title,
                Content = amendment.Content,
                OriginalContent = amendment.OriginalContent!,
                Justification = amendment.Justification,
                ParagrapheOrder = amendment.OrderParagraphe ?? 0
            };
            return amdVm;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentByIdAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "GetAmendmentByIdAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsListAsync(List<Guid> teamIds, Guid nodeId)
    {
        try
        {
            List<AmendmentsListVm> amendmentsList = new();
            List<Amendment> amendments = new List<Amendment>();
            Node[] nodes;
            var amendmentsNode = await _dbSet
                                          .AsNoTracking()
                                          .AsSingleQuery()
                                          .Where(a => a.NodeId == nodeId && a.IsDeleted == false && teamIds.Any(Id => Id == a.TeamId))
                                          .Include(t => t.Team)
                                          .Include(v => v.VoteAmendementResult)
                                          .Include(n => n.Node)
                                          .OrderBy(n => n.OrderParagraphe)
                                          .ToListAsync();
            if (amendmentsNode.Any())
            {
                amendmentsList = await SetAmendmentsList(amendmentsNode);
            }
            nodes = await _db.Nodes.Where(node => (node.ParentNodeId == nodeId)).OrderBy(n => n.Order).ToArrayAsync();
            if (nodes.Length == 0)
            {
                return amendmentsList;
            }
            foreach (var node in nodes)
            {
                amendmentsList.AddRange(await GetAmendmentsListAsync(teamIds, node.Id));
            }
            return amendmentsList;

        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsListAsync), nameof(AmendmnetRepository));
            // _logger.LogError(ex.Message, "GetAmendmentsListAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsListAsync(List<Guid> Ids)
    {
        try
        {
            List<AmendmentsListVm> amendments = new();
            if (Ids.Any())
            {

                amendments = await _dbSet
                                    .AsNoTracking()
                                    .AsSplitQuery()
                                    .Include(t => t.Team)
                                    .Include(p => p.GovernmentPositions)
                                    .Include(v => v.VoteAmendementResult)
                                    .Include(n => n.Node)
                                    .Where(a => Ids.Any(id => id == a.Id))
                                    .OrderBy(am => am.Node.Order)
                                    .ThenBy(am => am.Team.Ordre)
                                    .Select(amd => new AmendmentsListVm()
                                    {
                                        Id = amd.Id,
                                        Number = amd.Number,
                                        Order = amd.Ordre,
                                        NodeTitle = amd.Node.Label,
                                        TitleParagraphe = amd.TitreParagraphe,
                                        NodeId = amd.NodeId,
                                        NodeTypeId = amd.Node.TypeId,
                                        Title = amd.Title,
                                        AmendmentType = amd.AmendmentType,
                                        AmendmentIntent = Constants.GetAmendmentIntent(amd.AmendmentIntent),
                                        AmendmentsStatus = Constants.GetAmendmentStatu(amd.AmendmentsStatus),
                                        Team = amd.Team.Name,
                                        NumberSystem = amd.NumSystem,
                                        GovernmentPosition = amd.GovernmentPositions.FirstOrDefault(g => g.AmendmentId == amd.Id).Position,
                                        VoteResult = amd.VoteAmendementResult.Result,
                                        Content = amd.Content,
                                        OriginalContent = amd.OriginalContent,
                                        Justification = amd.Justification,
                                    }).ToListAsync();

            }

            return amendments;

        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsListAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "GetSubmitedAmendmentsListAsync function error", nameof(GetAmendmentsListAsync));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsListForVotingAsync(Guid nodeId)
    {
        try
        {
            List<AmendmentsListVm> amendmentsList = new();
            Node[] nodes;
            var amendmentsNode = await _dbSet
                                          .AsNoTracking()
                                          .AsSplitQuery()
                                          .Where(a => a.NodeId == nodeId && !a.IsDeleted && a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString())
                                          .Include(t => t.Team)
                                          .Include(a => a.VoteAmendementResult)
                                          .Include(n => n.Node)
                                          .ToListAsync();
            if (amendmentsNode.Any())
            {
                amendmentsList = await SetAmendmentsList(amendmentsNode);
            }
            nodes = await _db.Nodes.Where(node => (node.ParentNodeId == nodeId)).OrderBy(n => n.Order).ToArrayAsync();
            if (nodes.Length == 0)
            {
                return amendmentsList;
            }
            foreach (var node in nodes)
            {
                amendmentsList.AddRange(await GetAmendmentsListForVotingAsync(node.Id));
            }
            return amendmentsList;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsListForVotingAsync), nameof(AmendmnetRepository));
            //   _logger.LogError(ex.Message, "GetAmendmentsListAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsPublicAsync(Guid nodeId)
    {
        try
        {
            List<AmendmentsListVm> amendmentsList = new();
            Node[] nodes;
            var amendmentsNode = await _dbSet
                                .AsNoTracking()
                                .AsSplitQuery()
                                .Where(a => a.NodeId == nodeId && !a.IsDeleted && a.AmendmentType == AmendmentTypes.SINGLE.ToString() && (a.AmendmentsStatus == AmendmentsStatus.SUBMITTED.ToString() || a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString()))
                                .Include(t => t.Team)
                                .Include(a => a.AmendmentClusters)
                                .Include(a => a.VoteAmendementResult)
                                .Include(n => n.Node)
                                .ToListAsync();
            if (amendmentsNode.Any())
            {
                amendmentsList = await SetAmendmentsList(amendmentsNode);
            }
            nodes = await _db.Nodes.Where(node => (node.ParentNodeId == nodeId)).OrderBy(n => n.Order).ToArrayAsync();

            if (nodes.Length == 0)
            {
                return amendmentsList;
            }
            foreach (var node in nodes)
            {
                amendmentsList.AddRange(await GetAmendmentsPublicAsync(node.Id));
            }
            return amendmentsList;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsPublicAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "GetAmendmentsListAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetClusterAmendments(Guid AmendmentId)
    {
        try
        {
            var amendment = await _dbSet
                              .AsQueryable()
                              .AsNoTracking()
                              .Include(a => a.AmendmentClusters).ThenInclude(n => n.Node)
                              .Include(n => n.Team).ThenInclude(n => n.ParentTeams)
                              .FirstOrDefaultAsync(a => a.Id == AmendmentId);
            List<AmendmentsListVm> amendmentsListVms = new();
            if (amendment is Amendment)
            {
                if (amendment.AmendmentClusters != null)
                    amendmentsListVms = await SetAmendmentsList(amendment.AmendmentClusters.ToList());
            }
            return amendmentsListVms;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetClusterAmendments), nameof(AmendmnetRepository));
            // _logger.LogError(ex.Message, "GetAmendmentsListAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetLiftAmendments(List<Guid> teamIds, Guid LawId, Guid PhaseId)
    {
        try
        {
            var amendments = await _dbSet
                            .AsNoTracking()
                            .AsQueryable()
                            .Include(t => t.Team)
                            .Include(v => v.VoteAmendementResult)
                            .Include(n => n.Node)
                            .Where(a => a.Node.LawId == LawId &&
                                         a.Node.PhaseLawId == PhaseId &&
                                         teamIds.Any(idTeam => idTeam == a.TeamId) && a.IsDeleted == false)
                            .ToListAsync();
            List<Amendment> amendmentsWithVote = new();
            foreach (var amendment in amendments)
            {

                if (amendment.VoteAmendementResult != null)
                {
                    amendmentsWithVote.Add(amendment);
                }
            }
            amendmentsWithVote = amendmentsWithVote
                .Where(a =>
                 a.VoteAmendementResult != null &&
                 (a.VoteAmendementResult.Result!.ToUpper().Trim() == VoteResults.REJECT.ToString() ||
                 a.VoteAmendementResult.Result!.ToUpper().Trim() == VoteResults.PARTIAL.ToString() ||
                 a.VoteAmendementResult.Result!.ToUpper().Trim() == VoteResults.SUPPRESSED.ToString()))
                .ToList();

            List<Amendment> amends_ = new();
            foreach (var amend in amendmentsWithVote)
            {
                var amendment_ = await findAsync(a => a.CreatedFromId == amend.Id && a.IsDeleted == false && teamIds.Any(idTeam => idTeam == a.TeamId));
                if (amendment_ is null)
                {
                    amends_.Add(amend);
                }

            }
            return await SetAmendmentsList(amends_);
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetLiftAmendments), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "GetLiftAmendments function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetSubmitedAmendmentsListAsync(Guid nodeId)
    {
        try
        {
            List<AmendmentsListVm> amendmentsList = new();
            List<Amendment> amendments = new List<Amendment>();
            Node[] nodes;
            var amendmentsNode = await _dbSet
                                .AsNoTracking()
                                .AsSingleQuery()
                                .Where(a => a.NodeId == nodeId && !a.IsDeleted && (a.AmendmentsStatus == AmendmentsStatus.SUBMITTED.ToString() || a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString()))
                                .Include(t => t.Team)
                                .Include(p => p.GovernmentPositions)
                                .Include(v => v.VoteAmendementResult)
                                .Include(n => n.Node)
                                .OrderBy(t => t.Team.Ordre)
                                .ToListAsync();
            if (amendmentsNode.Any())
                amendmentsList = await SetAmendmentsList(amendmentsNode);
            nodes = await _db.Nodes.Where(node => (node.ParentNodeId == nodeId)).OrderBy(n => n.Order).ToArrayAsync();
            if (nodes.Length == 0)
                return amendmentsList;
            foreach (var node in nodes)
            {
                amendmentsList.AddRange(await GetSubmitedAmendmentsListAsync(node.Id));
            }
            return amendmentsList;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetSubmitedAmendmentsListAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "GetSubmitedAmendmentsListAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> SetAmendmentsAsync(SetAmendmentStatuVm amendmentStatuVm)
    {
        try
        {
            bool isSuccess = false;
            foreach (var Id in amendmentStatuVm.Ids)
            {
                var amendment = await findAsync(a => a.Id == Id);
                if (amendment is Amendment)
                {
                    amendment.AmendmentsStatus = amendmentStatuVm.AmendmentStatu;
                    amendment.LastModifiedBy = amendmentStatuVm.UserId;
                    amendment.ModifictationDate = DateTime.UtcNow;
                    if (amendmentStatuVm.AmendmentStatu == AmendmentsStatus.SUBMITTED.ToString())
                    {
                        amendment.SubmitedDate = DateTime.UtcNow;
                        amendment.SubmitedBy = amendmentStatuVm.UserId;
                    }
                    if (amendmentStatuVm.AmendmentStatu == AmendmentsStatus.PUBLIC.ToString())
                    {
                        amendment.PublishedDate = DateTime.UtcNow;
                        amendment.PublishedBy = amendmentStatuVm.UserId;
                    }
                    _dbSet.Entry(amendment).State = EntityState.Modified;
                    isSuccess = await _db.SaveChangesAsync() > 0;
                }
            }
            return new ServerResponse(isSuccess, isSuccess ? "success" : "failed");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(SetAmendmentsAsync), nameof(AmendmnetRepository));
            // _logger.LogError(ex.Message, "Error on set amendment statu", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> UpdateAmendmantAsync(Guid amendmentId, AmendmentVm amendmentVm)
    {
        try
        {
            var amendment = await _dbSet.Include(a => a.AmendmentClusters).FirstOrDefaultAsync(a => a.Id == amendmentId);
            if (amendment == null) return new ServerResponse(false, $"Amendment where Id :{amendmentId}");
            //amendment.Number = amendmentVm.Number;
            amendment.AmendmentIntent = amendmentVm.AmendmentIntent;
            amendment.Title = amendmentVm.Title;
            amendment.Content = amendmentVm.Content;
            amendment.Justification = amendmentVm.Justification;
            amendment.OriginalContent = amendmentVm.OriginalContent;
            amendment.ModifictationDate = DateTime.Now;
            amendment.LastModifiedBy = amendmentVm.UpdatedBy;
            amendment.OrderParagraphe = amendmentVm.ParagrapheOrder;
            amendment.TitreParagraphe = amendmentVm.ParagreapheTitle;
            _dbSet.Entry(amendment).State = EntityState.Modified;
            if (await _db.SaveChangesAsync() > 0)
            {
                if (amendment.AmendmentType != AmendmentTypes.SINGLE.ToString())
                {
                    if (amendmentVm.AmendmentIds != null)
                    {
                        foreach (var amd in amendment.AmendmentClusters.ToList())
                        {
                            amendment.AmendmentClusters.Remove(amd);
                            await _db.SaveChangesAsync();
                        }
                        foreach (var Id in amendmentVm.AmendmentIds)
                        {
                            amendment.AmendmentClusters.Add(await GetByIdAsync(Id));
                            await _db.SaveChangesAsync();
                        }
                    }
                }
                return new ServerResponse(true, "Updated");
            }
            return new ServerResponse(false, "error on update");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(UpdateAmendmantAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "UpdateAmendmantAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    private async Task<List<AmendmentsListVm>> SetAmendmentsList(List<Amendment> amendments)
    {
        List<AmendmentsListVm> amends = new();
        var firstAmendment = amendments.First();
        var node = await _db.Nodes.Include(n => n.Law).FirstOrDefaultAsync(l => l.Id == firstAmendment.NodeId);
        foreach (var amend in amendments)
        {
            AmendmentsListVm amendmentsList = new();
            amendmentsList.Id = amend.Id;
            amendmentsList.Number = amend.Number;
            amendmentsList.Order = amend.Ordre;
            amendmentsList.NodeTitle = node?.Law.Category == "PLF" ? GetNodeLabel(amend.Node!) + " - " + amend.TitreParagraphe : amend.Node!.Label;
            if (amend.Node.ParentNode != null)
            {
                amendmentsList.ParentNodeTitle = node?.Law.Category == "PLF" ? GetNodeLabel(amend.Node.ParentNode) : amend.Node!.Label;
            }
            //amendmentsList.Content = amend.Content;
            //amendmentsList.OriginalContent = amend.OriginalContent;
            //amendmentsList.Justification = amend.Justification;
            amendmentsList.NodeId = amend.NodeId;
            amendmentsList.Title = amend.Title;
            amendmentsList.AmendmentType = amend.AmendmentType;
            amendmentsList.AmendmentIntent = Constants.GetAmendmentIntent(amend.AmendmentIntent);
            amendmentsList.AmendmentsStatus = Constants.GetAmendmentStatu(amend.AmendmentsStatus);
            amendmentsList.Team = (await _db.Teams.FirstOrDefaultAsync(a => a.Id == amend.TeamId))?.Name;
            amendmentsList.NumberSystem = amend.NumSystem;
            amendmentsList.CreationDate = amend.CreationDate;
            amendmentsList.SubmitedDate = amend.SubmitedDate;
            amendmentsList.VoteResult = Constants.GetVoteResult(amend.VoteAmendementResult?.Result ?? string.Empty);
            amendmentsList.VotingDate = amend!.VoteAmendementResult?.CreationDate;
            amendmentsList.IsAmendmentSession = amend.IsAmendementSeance ?? false;
            amendmentsList.IsAmendementRattrape = amend.IsAmendementRattrape ?? false;
            amends.Add(amendmentsList);
        }
        return amends;
    }
    private AmendmentsListVm SetAmendmentsListVm(Amendment amendments)
    {
        return new AmendmentsListVm()
        {
            Id = amendments.Id,
            Number = amendments.Number,
            Order = amendments.Ordre,
            NodeTitle = amendments.Node?.Label,
            Title = amendments.Title,
            NumberSystem = amendments.NumSystem,
            AmendmentType = amendments.AmendmentType,
            AmendmentIntent = Constants.GetAmendmentIntent(amendments.AmendmentIntent),
            AmendmentsStatus = Constants.GetAmendmentStatu(amendments.AmendmentsStatus),
            Team = amendments.Team.Name
        };
    }
    public async Task<List<Amendment>> GetLiftAmendmentsListAsync(Guid teamId, Guid nodeId)
    {
        List<Amendment> amendmentsList = new();
        Node[] nodes;
        List<Amendment> amendmentsNode = new();
        amendmentsNode = await _dbSet
                        .AsNoTracking()
                        .AsSplitQuery()
                        .Include(t => t.Team)
                        .Include(v => v.VoteAmendementResult)
                        .Include(n => n.Node)
                        .Where(a => a.NodeId == nodeId && !a.IsDeleted && a.TeamId == teamId && a.CreatedFromId == null)
                        .ToListAsync();

        if (amendmentsNode.Any())
            amendmentsList = amendmentsNode;
        nodes = await _db.Nodes.Where(node => (node.ParentNodeId == nodeId)).OrderBy(n => n.Order).ToArrayAsync();
        if (nodes.Length == 0)
            return amendmentsList;
        foreach (var node in nodes)
            amendmentsList.AddRange(await GetLiftAmendmentsListAsync(teamId, node.Id));
        return amendmentsList;

    }
    public async Task<ServerResponse> CloneAmendmentsAsync(CloneAmendmentsVm amendments)
    {
        try
        {
            bool result = false;
            foreach (var Id in amendments.Ids)
            {
                var amendment = await findAsync(a => a.Id == Id);
                if (amendment is Amendment)
                {
                    var newAmendment = new Amendment();
                    newAmendment.Id = Guid.NewGuid();
                    newAmendment.Number = amendment.Number;
                    newAmendment.TeamId = amendment.TeamId;
                    newAmendment.Supressed = false;
                    newAmendment.AmendmentType = amendment.AmendmentType;
                    newAmendment.AmendmentIntent = amendment.AmendmentIntent;
                    newAmendment.AmendmentsStatus = AmendmentsStatus.EDITABLE.ToString();
                    newAmendment.Content = amendment.Content;
                    newAmendment.Article = amendment.Article;
                    newAmendment.Title = amendment.Title;
                    newAmendment.Justification = amendment.Justification;
                    newAmendment.ArticleRef = amendment.ArticleRef;
                    newAmendment.OriginalContent = amendment.OriginalContent;
                    newAmendment.ReferenceNodeId = amendment.NodeId;
                    newAmendment.CreatedFromId = amendment.Id;
                    newAmendment.Ordre = amendment.Ordre;
                    newAmendment.IsDeleted = false;
                    newAmendment.CreatedBy = amendments.UserId;
                    newAmendment.CreationDate = DateTime.UtcNow;
                    var node = await _db.Nodes.FirstOrDefaultAsync(a => a.CreatedFrom == amendment.NodeId);
                    if (node is Node)
                        newAmendment.NodeId = node.Id;
                    await _db.Amendments.AddAsync(newAmendment);
                    result = await _db.SaveChangesAsync() > 0;
                }
            }
            return new ServerResponse(result, result ? "Success" : "Faild");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(CloneAmendmentsAsync), nameof(AmendmnetRepository));
            //  _logger.LogError(ex.Message, "", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetPublicAmendmentsListAsync(Guid nodeId)
    {
        try
        {
            List<AmendmentsListVm> amendmentsList = new();
            List<Amendment> amendments = new List<Amendment>();
            Node[] nodes;
            var amendmentsNode = await _dbSet
                                          .AsNoTracking()
                                          .AsSplitQuery()
                                          .Where(a => a.NodeId == nodeId && !a.IsDeleted && a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString())
                                          .Include(t => t.Team)
                                          .Include(p => p.GovernmentPositions)
                                          .Include(v => v.VoteAmendementResult)
                                          .Include(n => n.Node)
                                          .ToListAsync();
            if (amendmentsNode.Any())
                amendmentsList = await SetAmendmentsList(amendmentsNode);
            nodes = await _db.Nodes.Where(node => (node.ParentNodeId == nodeId)).OrderBy(n => n.Order).ToArrayAsync();
            if (nodes.Length == 0)
                return amendmentsList;
            foreach (var node in nodes)
            {
                amendmentsList.AddRange(await GetPublicAmendmentsListAsync(node.Id));
            }
            return amendmentsList;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetPublicAmendmentsListAsync), nameof(AmendmnetRepository));
            // _logger.LogError(ex.Message, "GetPublicAmendmentsListAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<AmendmentDetails> GetAmendmentDetailsAsync(Guid Id)
    {
        try
        {
            var amendment = await _db.Amendments.Include(am => am.Team).FirstOrDefaultAsync(am => am.Id == Id);
            if (amendment is null) return null!;
            var vote = await _db.VoteAmendementResults.FirstOrDefaultAsync(am => am.AmendmentId == Id);
            var amendmentDetailsdVm = new AmendmentDetails();
            amendmentDetailsdVm.TeamName = amendment.Team.Name;
            amendmentDetailsdVm.Number = amendment.Number;
            amendmentDetailsdVm.AmendmentIntent = Constants.GetAmendmentIntent(amendment.AmendmentIntent);
            amendmentDetailsdVm.Title = amendment.Title;
            amendmentDetailsdVm.Content = amendment.Content;
            amendmentDetailsdVm.OriginalContent = amendment.OriginalContent!;
            amendmentDetailsdVm.Justification = amendment.Justification;
            amendmentDetailsdVm.InFavor = vote?.InFavor;
            amendmentDetailsdVm.Against = vote?.Against;
            amendmentDetailsdVm.Neutral = vote?.Neutral;
            amendmentDetailsdVm.Result = Constants.GetVoteResult(vote?.Result ?? string.Empty);
            amendmentDetailsdVm.Observation = vote?.Observation;
            amendmentDetailsdVm.NodeId = amendment.NodeId;
            return amendmentDetailsdVm;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentDetailsAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "GetAmendmentDetailsAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetNodeAmendmentsAsync(Guid NodeId)
    {
        try
        {
            string REJECT = Enum.GetName(typeof(VoteResults), VoteResults.REJECT)!;
            string SUPPRESSED = Enum.GetName(typeof(VoteResults), VoteResults.SUPPRESSED)!;

            var amendments = await _dbSet
            .Include(a => a.Team)
            .Where(amd => amd.NodeId == NodeId && !amd.IsDeleted && amd.VoteAmendementResult!.Result != REJECT && amd.VoteAmendementResult.Result != SUPPRESSED)
            .Select(am => new AmendmentsListVm
            {
                Id = am.Id,
                Number = am.Number,
                Order = am.Ordre,
                Title = am.Title,
                NumberSystem = am.NumSystem,
                Team = am.Team.Name,
                Content = am.Content,
                NewContent = am.NewContent
            })
            .ToListAsync();
            return amendments;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetNodeAmendmentsAsync), nameof(AmendmnetRepository));
            // _logger.LogError(ex.Message, "GetPublicAmendmentsListAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> SetNewContent(SetContent model)
    {
        try
        {
            foreach (var Id in model.Ids)
            {
                var _amendmentContent = await _db.AmendmentContents.FirstOrDefaultAsync(amd => amd.AmendmentId == Id);
                if (_amendmentContent != null)
                {
                    _db.Remove(_amendmentContent);
                    await _db.SaveChangesAsync();
                }
            }
            foreach (var Id in model.Ids)
            {
                var amendment = await findAsync(a => a.Id == Id);
                amendment.NewContent = model.Content;
                _db.Entry(amendment).State = EntityState.Modified;
                await _db.AmendmentContents.AddAsync(new AmendmentContent
                {
                    AmendmentId = amendment.Id,
                    Content = amendment.Content,
                });
                await _db.SaveChangesAsync();
            }
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(SetNewContent), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, $"{nameof(SetNewContent)} function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetAllSubmitedAmendments(Guid NodeId)
    {
        try
        {
            var node = await _db.Nodes.FirstOrDefaultAsync(n => n.Id == NodeId && !n.IsDeleted);
            if (node == null)
            {
                return null!;
            }
            var amendments = await GetAmendments(node.Id);
            return await SetAmendmentsList(amendments);
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAllSubmitedAmendments), nameof(AmendmnetRepository));
            // _logger.LogError(ex.Message, $"", nameof(GetAllSubmitedAmendments));
            throw;
        }
    }
    public async Task<List<Amendment>> GetAmendments(Guid nodeId)
    {
        List<Amendment> amendments = new List<Amendment>();
        Node[] nodes;

        var amendmentsNode = await _dbSet
                                      .AsNoTracking()
                                      .Where(a => a.NodeId == nodeId && !a.IsDeleted && a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString() && !(bool)a.IsAmendementRattrape)
                                      .Include(t => t.Team)
                                      .Include(a => a.AmendmentClusters)
                                      .Include(a => a.VoteAmendementResult)
                                      .Include(n => n.Node)
                                      .Include(n => n.Node.ParentNode)
                                      .OrderBy(n => n.Team.Ordre)
                                      .ToListAsync();
        if (amendmentsNode.Any())
        {
            amendments = amendmentsNode;
        }
        nodes = await _db.Nodes.Where(node => node.ParentNodeId == nodeId).OrderBy(n => n.Order).ToArrayAsync();

        if (nodes.Length == 0)
        {
            return amendments;
        }
        foreach (var node in nodes)
        {
            amendments.AddRange(await GetAmendments(node.Id));
        }
        return amendments;


    }
    public string GetNodeLabel(Node node)
    {
        var nodeType = _db.NodeTypes.FirstOrDefault(t => t.Id == node.TypeId);
        return Constants.GetNodelabel(nodeType?.Label!, node.Number, node.Label, node.Bis);
    }
    public async Task<ServerResponse> SetAmendmentsOrders(SetAmendmentOrder model)
    {
        try
        {
            foreach (var amd in model.AmendmentOrders)
            {
                var amendment = await findAsync(am => am.Id == amd.Id);
                amendment.Ordre = amd.Order;
                amendment.LastModifiedBy = model.LastModifiedBy;
                amendment.ModifictationDate = DateTime.UtcNow;
                _db.Entry(amendment).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(SetAmendmentsOrders), nameof(AmendmnetRepository));
            // _logger.LogError(ex.Message, $"Error On {nameof(SetAmendmentsOrders)}", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetAllSubmitedAmendments(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var node = await _db.Nodes.FirstOrDefaultAsync(n => n.LawId == LawId && n.PhaseLawId == PhaseLawId && !n.IsDeleted && n.ParentNodeId == null);
            if (node == null)
            {
                return null!;
            }
            var amendments = await GetAmendments(node.Id);
            return await SetAmendmentsList(amendments);
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAllSubmitedAmendments), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, $"", nameof(GetAllSubmitedAmendments));
            throw;
        }

    }
    public async Task<ServerResponse> CheckAmendmentsHasNewContent(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var nodes = await _db.Nodes.Where(node => node.LawId == LawId && node.PhaseLawId == PhaseLawId && !node.IsDeleted).ToListAsync();
            List<Guid> Ids = nodes.Select(n => n.Id).ToList();
            var amendments = await _dbSet.AsNoTracking()
                 .AsSingleQuery()
                 .Where(amd => Ids.Any(a => a == amd.NodeId) && !amd.IsDeleted && amd.VoteAmendementResult!.Result != null && amd.VoteAmendementResult.Result != VoteResults.REJECT.ToString() && amd.VoteAmendementResult.Result != VoteResults.SUPPRESSED.ToString())
                 .ToListAsync();
            if (amendments.Count == 0)
            {
                return new ServerResponse(false, "ok");
            }
            var check = amendments.Any(am => (string.IsNullOrEmpty(am.NewContent)));
            return new ServerResponse(check, check ? "المرجو تححين الصيغة قبل نسخ العقدة" : "ok");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(CheckAmendmentsHasNewContent), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, $"Error On {CheckAmendmentsHasNewContent}", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> CheckAmendmentsSectionHasNewContent(Guid nodeId)
    {

        try
        {
            var section = await _db.Nodes.FirstOrDefaultAsync(n => n.Id == nodeId);
            var nodes = await _db.Nodes.Where(node => node.LawId == section!.LawId && node.PhaseLawId == section.PhaseLawId && !node.IsDeleted).ToListAsync();
            List<Guid> Ids = nodes.Select(n => n.Id).ToList();
            var amendments = await GetSectionAmendments(nodeId);
            if (amendments.Count == 0)
            {
                return new ServerResponse(false, "ok");
            }
            bool check = amendments.Any(am => (string.IsNullOrEmpty(am.NewContent)));
            return new ServerResponse(check, check ? "المرجو تححين الصيغة قبل نسخ العقدة" : "ok");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(CheckAmendmentsSectionHasNewContent), nameof(AmendmnetRepository));
            // _logger.LogError(ex.Message, $"Error On {CheckAmendmentsHasNewContent}", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsForCluster(Guid NodeId)
    {
        try
        {
            var amds = await _dbSet
                .AsNoTracking()
                .AsSingleQuery()
                .Where(am => am.NodeId == NodeId && !am.IsDeleted && am.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString())
                .Include(am => am.Amendments)
                .Include(am => am.Node)
                .ToListAsync();
            return await SetAmendmentsList(amds.Where(a => a.Amendments.Count == 0).ToList());
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsForCluster), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, $"Error On {CheckAmendmentsHasNewContent}", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> insertGovernmentAmendmentsAsync(List<AmendmentDto> amendments)
    {
        try
        {
            List<Amendment> _amendments = new List<Amendment>();
            // var team = await _db.Teams.FirstOrDefaultAsync(t => t.TeamType == "GOVERNEMENTS");
            foreach (var amendment in amendments)
            {
                Amendment newAmendment = new();
                Guid NodeID = Guid.Empty;
                if (amendment.AmendmentIntent.ToString() == AmendmentIntents.ADDITION.ToString())
                {
                    
                    var existNode = await _db.Nodes.FirstOrDefaultAsync(node =>node.IdFinance==amendment.NodeId);
                    if (existNode != null)
                    {
                        NodeID = existNode.Id;
                    }
                    else
                    {
                        var parentNode = await _db.Nodes.FirstOrDefaultAsync(n => n.IdFinance == amendment.Node.ParentNode!.Id);
                        var virtuelNode = new Node();
                        virtuelNode.Label = amendment.Node!.Label!;
                        virtuelNode.TypeId = amendment.Node.TypeId;
                        virtuelNode.Content = amendment.Node.Content!;
                        virtuelNode.OriginalContent = amendment.Node.OriginalContent!;
                        virtuelNode.PresentationNote = amendment.Node.PresentationNote;
                        virtuelNode.Counter = 0;//TODO
                        virtuelNode.PhaseLawId = parentNode!.PhaseLawId;
                        virtuelNode.LawId = parentNode!.LawId;
                        virtuelNode.Checked = false;
                        virtuelNode.Number = amendment.Node.Number;
                        virtuelNode.Bis = amendment.Node.Bis;
                        virtuelNode.Order = amendment.Node.Order;
                        virtuelNode.IsDeleted = false;
                        virtuelNode.CreatedBy = Guid.Parse("F37BE3F3-9E2E-4E64-90EF-DCF48F38BF48");
                        virtuelNode.CreationDate = DateTime.UtcNow;
                        virtuelNode.IdFinance = amendment.Node.Id;
                        virtuelNode.Status = amendment.Node.Status.ToString()!;
                        virtuelNode.ParentNodeId = parentNode.Id;
                        virtuelNode.IdFinance = amendment.Node.Id;
                        await _db.Nodes.AddAsync(virtuelNode);
                        await _db.SaveChangesAsync();
                        NodeID = virtuelNode.Id;
                    }
                  
                }
                else
                {
                    var node = await _db.Nodes.FirstOrDefaultAsync(a => a.IdFinance == amendment.NodeId);
                    NodeID = node!.Id;
                }
                if (NodeID != Guid.Empty)
                {
                    newAmendment.NodeId = NodeID;
                    newAmendment.TeamId = amendment.TeamId;
                    newAmendment.Number = amendment.Number;
                    newAmendment.Supressed = false;
                    newAmendment.AmendmentType = amendment.AmendmentType.ToString();
                    newAmendment.AmendmentIntent = amendment.AmendmentIntent.ToString();
                    newAmendment.AmendmentsStatus = AmendmentsStatus.PUBLIC.ToString();
                    newAmendment.SubmitedDate = DateTime.UtcNow;
                    newAmendment.Title = amendment.Title;
                    newAmendment.Content = amendment.Content;
                    newAmendment.Justification = amendment.Justification;
                    newAmendment.OriginalContent = amendment.OriginalContent;
                    newAmendment.ArticleRef = string.Empty;
                    newAmendment.CreatedBy = Guid.Parse("B07A83AA-DE61-4DA0-4F3D-08DAB5CBF849");
                    newAmendment.CreationDate = DateTime.UtcNow;
                    newAmendment.IdFinance = amendment.Id;
                    newAmendment.OrderParagraphe = 1;
                    newAmendment.TitreParagraphe = "فقرة 1";
                    newAmendment.IsAmendementRattrape = false;
                    newAmendment.IsAmendementSeance = false;
                    var existeAmendment = await findAsync(am => am.IdFinance == amendment.Id);
                    if (existeAmendment is null)
                    {
                        _amendments.Add(newAmendment);
                    }
                }
            }
            if (_amendments.Count > 0)
            {
                await _dbSet.AddRangeAsync(_amendments);
                bool result = await _db.SaveChangesAsync() > 0;
                return new ServerResponse(result, result ? "Success" : "Fail");
            }
            return new ServerResponse(false, "No Amendments for inserted");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(insertGovernmentAmendmentsAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, $"Error when caling {insertGovernmentAmendmentsAsync}", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentDto>> GetAmendmentsAsync(List<Guid> TeamIds, Guid lawId, Guid PhaseLawId)
    {
        try
        {
            var amendments = await _db.Amendments
                .Include(am => am.Node)
                .Include(am => am.Node.ParentNode)
                .Where(a => TeamIds.Contains(a.TeamId) &&
                      a.Node.PhaseLawId == PhaseLawId &&
                      (a.AmendmentsStatus == AmendmentsStatus.SUBMITTED.ToString() || a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString()) && !a.IsDeleted)
             .ToArrayAsync();

            List<AmendmentDto> amendmentDtos = new List<AmendmentDto>();

            foreach (var am in amendments)
            {
                AmendmentDto amendmentDto = new AmendmentDto();
                amendmentDto.Id = am.Id;
                amendmentDto.NodeId = am.Node.IdFinance ?? Guid.Empty;
                amendmentDto.TeamId = am.TeamId;
                amendmentDto.Number = am.Number;
                amendmentDto.AmendmentType = (AmendmentTypes)Enum.Parse(typeof(AmendmentTypes), am.AmendmentType);
                amendmentDto.AmendmentIntent = (AmendmentIntents)Enum.Parse(typeof(AmendmentIntents), am.AmendmentIntent);
                amendmentDto.AmendmentsStatus = (AmendmentsStatus)Enum.Parse(typeof(AmendmentsStatus), am.AmendmentsStatus);
                amendmentDto.Title = am.Title;
                amendmentDto.Content = am.Content;
                amendmentDto.Justification = am.Justification;
                amendmentDto.OriginalContent = am.OriginalContent ?? string.Empty;
                amendmentDto.Ordre = am.Ordre;
                amendmentDto.Node = SetNodeDto(am.Node);
                amendmentDto.amendmentsRefIds = am.AmendmentClusters.Select(x => x.Id.ToString()).ToArray();
                amendmentDtos.Add(amendmentDto);
            }
            return amendmentDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error when caling {GetAmendmentsAsync}", nameof(AmendmnetRepository));
            return new List<AmendmentDto>();
        }
    }
    DataAccess.Dtos.NodeDto SetNodeDto(Node node) => new DataAccess.Dtos.NodeDto()
    {
        Id = (Guid)node.IdFinance,
        Label = node.Label,
        TypeId = node.TypeId,
        Content = node.Content,
        OriginalContent = node.OriginalContent,
        PresentationNote = node.PresentationNote ?? string.Empty,
        ParentNodeId = node.ParentNode?.IdFinance,
        Status = (NodeStatus)Enum.Parse(typeof(NodeStatus), node.Status.ToString()),
        Number = node.Number,
        Bis = node.Bis,
        Order = node.Order,
        IsDeleted = node.Status == NodeStatus.VIRTUAL.ToString() ? true : false,
        Nature = (node.Nature != string.Empty && node.Nature != null) ? (NodeOrigin)Enum.Parse(typeof(NodeOrigin), node.Nature!.ToString()) : null,
        ParentNode = new DataAccess.Dtos.NodeDto()
        {
            //Id = node.ParentNode.Id,
            Id = (Guid)node.ParentNode.IdFinance,
            Label = node.ParentNode.Label,
            TypeId = node.ParentNode.TypeId,
            Content = node.ParentNode.Content,
            OriginalContent = node.ParentNode.OriginalContent,
            PresentationNote = node.ParentNode.PresentationNote ?? string.Empty,
            ParentNodeId = node.ParentNode.ParentNode?.IdFinance,
            Status = (NodeStatus)Enum.Parse(typeof(NodeStatus), node.ParentNode.Status.ToString()),
            Number = node.ParentNode.Number,
            Bis = node.ParentNode.Bis,
            Order = node.ParentNode.Order,
            IsDeleted = node.ParentNode.IsDeleted,
            Nature = (node.ParentNode.Nature != string.Empty && node.ParentNode.Nature != null) ? (NodeOrigin)Enum.Parse(typeof(NodeOrigin), node.ParentNode.Nature!.ToString()) : null,
        }

    };
    public async Task<List<VoteAmendment>> GetVoteAmendmentsAsync(Guid NodeId)
    {
        try
        {
            var node = await _db.Nodes.FirstOrDefaultAsync(node => node.Id == NodeId);
            if (node is null)
            {
                return new List<VoteAmendment>();
            }
            var amendments = (await GetAmendments(node.Id)).OrderBy(a => a.Node.Order).ThenBy(a => a.Team.Weight).ThenBy(a => a.Ordre).ToList();

            return amendments.Select(amd => new VoteAmendment
            {
                Id = amd.VoteAmendementResult.Id,
               // IdFinance = amd.Team.Name == "" ? (Guid)amd.IdFinance! : amd.Id,
                IdFinance = amd.IdFinance !=null ? amd.IdFinance:amd.Id,
                NodeId = amd.Node.IdFinance ?? Guid.Empty,
                Infavor = amd.VoteAmendementResult?.InFavor,
                Against = amd.VoteAmendementResult?.Against,
                Neutral = amd.VoteAmendementResult?.Neutral,
                Result = amd.VoteAmendementResult?.Result ?? string.Empty,
                Observation = amd.VoteAmendementResult?.Observation ?? string.Empty,
                suppressed = amd.VoteAmendementResult?.Suppressed ?? false,
            }).ToList();
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetVoteAmendmentsAsync), nameof(AmendmnetRepository));
            //   _logger.LogError(ex.Message, $"Error when caling {GetVoteAmendmentsAsync}", nameof(AmendmnetRepository));
            return new List<VoteAmendment>();
        }
    }
    public async Task<List<Amendment>> GetSectionAmendments(Guid nodeId)
    {
        try
        {
            List<Amendment> amendmentsList = new();
            Node[] nodes;
            var amendmentsNode = await _dbSet
                                          .AsNoTracking()
                                          .AsSplitQuery()
                                          .Where(a => a.NodeId == nodeId && !a.IsDeleted && a.VoteAmendementResult!.Result != null && a.VoteAmendementResult.Result != VoteResults.REJECT.ToString() && a.VoteAmendementResult.Result != VoteResults.SUPPRESSED.ToString())
                                          .Include(a => a.VoteAmendementResult)
                                          .Include(a => a.AmendmentContents)
                                          .ToListAsync();
            if (amendmentsNode.Any())
            {
                amendmentsList = amendmentsNode;
            }
            nodes = await _db.Nodes.Where(node => (node.ParentNodeId == nodeId)).OrderBy(n => n.Order).ToArrayAsync();
            if (nodes.Length == 0)
            {
                return amendmentsList;
            }
            foreach (var node in nodes)
            {
                amendmentsList.AddRange(await GetSectionAmendments(node.Id));
            }
            return amendmentsList;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetSectionAmendments), nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> CheckAmendmentExisteByNumberAsync(Guid LawId, Guid phaseId, List<Guid> TeamIds, int Number)
    {
        try
        {
            var nodeIds = await _db.Nodes
                        .AsNoTracking()
                        .AsSingleQuery()
                        .Where(node => node.LawId == LawId && node.PhaseLawId == phaseId && !node.IsDeleted)
                        .Select(node => node.Id)
                        .ToListAsync();
            var amendment = await findAsync(amd => nodeIds.Any(nodeId => nodeId == amd.NodeId) && TeamIds.Any(id => id == amd.TeamId) && amd.Number == Number && !amd.IsDeleted);
            if (amendment is Amendment)
                return new ServerResponse(true, $"Amendment Number already exisit by number-{Number}");
            return new ServerResponse(false, "Amendment not exist");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(CheckAmendmentExisteByNumberAsync), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, "CheckAmendmantExisteAsync function error", nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<List<AmendmentsListVm>> GetAmendmentsForCluster(Guid LawId, Guid PhaseLawId)
    {
        try
        {
            var node = await _db.Nodes.FirstOrDefaultAsync(node => node.LawId == LawId && node.PhaseLawId == PhaseLawId && !node.IsDeleted && node.ParentNodeId == null);
            if (node is null)
            {
                return new List<AmendmentsListVm>();
            }
            var amendments = (await GetAmendments(node.Id)).OrderBy(a => a.Node.Order).ThenBy(a => a.Team.Weight).ThenBy(a => a.Ordre).ToList();
            return await SetAmendmentsList(amendments);
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsForCluster), nameof(AmendmnetRepository));
            //_logger.LogError(ex.Message, $"Error when caling GetAmendmentsForCluster", nameof(AmendmnetRepository));
            return new List<AmendmentsListVm>();
        }
    }
    public async Task<List<CountAmendmentDto>> CountAmendmentsByTeamAndLaw(Guid LawId, Guid PhaseId)
    {
        try
        {
            var node = await _db.Nodes.FirstOrDefaultAsync(node => node.LawId == LawId && node.PhaseLawId == PhaseId && node.ParentNode == null);
            if (node is null)
            {
                return new List<CountAmendmentDto>();
            }
            var amendments = (await GetAmendments(node.Id)).OrderBy(a => a.Node.Order).ThenBy(a => a.Team.Weight).ThenBy(a => a.Ordre).ToList();
            List<CountAmendmentDto> countAmendments = new List<CountAmendmentDto>();
            var teams = await _db.Teams.Where(team => team.IsDeleted == false).ToListAsync();
            foreach (var team in teams)
            {
                var amendments_ = amendments.Where(amd => amd.TeamId == team.Id).OrderByDescending(amd => amd.SubmitedDate).ToList();
                if (amendments_.Count > 0)
                {
                    var countAmendment = new CountAmendmentDto();
                    countAmendment.Count = amendments_.Count;
                    countAmendment.TeamName = team.Name;
                    countAmendment.SubmitedDate = amendments_.First().SubmitedDate;
                    countAmendment.PuplishedDate = amendments_.First().PublishedDate;
                    var userSubmited = await _db.Users.FindAsync(amendments_.First().SubmitedBy);
                    countAmendment.SubmitedBy = userSubmited?.FirstName + " " + userSubmited?.LastName;
                    var userPublished = await _db.Users.FindAsync(amendments_.First().PublishedBy);
                    countAmendment.PuplishedBy = userPublished?.FirstName + " " + userPublished?.LastName;
                    var vote = amendments_.First()?.VoteAmendementResult;
                    if (vote != null)
                    {
                        countAmendment.VotingDate = vote.CreationDate;
                        var userVoting = await _db.Users.FindAsync(vote.CreatedBy);
                        countAmendment.VotingBy = userVoting?.FirstName + " " + userVoting?.LastName;
                    }
                    countAmendments.Add(countAmendment);
                }
            }
            return countAmendments;

        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(CountAmendmentsByTeamAndLaw), nameof(AmendmnetRepository));
            return new List<CountAmendmentDto>();
        }
    }
    private async Task RecalculateOrderOnDeletetNode(Node deletedNode)
    {
        var nodes = await _db.Nodes.Where(nd => nd.ParentNodeId == deletedNode.ParentNodeId).OrderBy(o => o.Order).ToListAsync();
        int order = 0;
        foreach (var node in nodes)
        {
            order++;
            node.Order = order;
            await _db.SaveChangesAsync();
        }
    }
    public async Task<List<Amendment>> GetAmendmentsForVotingFile(Guid NodeId)
    {
        try
        {
            List<Amendment> amendments = new List<Amendment>();
            Node[] nodes;
            var amendmentsNode = await _dbSet
                                          .AsNoTracking()
                                          .AsSplitQuery()
                                          .Where(a => a.NodeId == NodeId && !a.IsDeleted && a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString() && a.Node.Type.Label != "الباب")
                                          .Include(t => t.Team)
                                          .Include(v => v.VoteAmendementResult)
                                          .ToListAsync();
            if (amendmentsNode.Any())
                amendments = amendmentsNode;
            nodes = await _db.Nodes.Include(nd => nd.Type).Where(node => (node.ParentNodeId == NodeId)).OrderBy(n => n.Order).ToArrayAsync();
            if (nodes.Length == 0)
                return amendments;
            foreach (var node in nodes)
            {
                if (node.Type.Label.Trim() != "الفقرة")
                {
                    amendments.AddRange(await GetAmendmentsForVotingFile(node.Id));
                }
            }
            return amendments;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsForVotingFile), nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> ReassignmentAmendment(Guid AmendmentId, Guid NodeId, Guid LastModifiedBy)
    {
        try
        {
            var result = false;
            var node = await _db.Nodes.Include(nd => nd.Type).FirstOrDefaultAsync(nd => nd.Id == NodeId);
            var amendment = await _dbSet.Include(amd => amd.Node).FirstOrDefaultAsync(amd => amd.Id == AmendmentId);
            if (amendment is null)
                return new ServerResponse(false, $"Amendment where Id={AmendmentId} not found");
            var OldAmdNodeId = amendment.NodeId;
            var oldNode = amendment.Node;
            if (amendment.AmendmentIntent != AmendmentIntents.ADDITION.ToString())
            {
                amendment.OldNodeId = OldAmdNodeId;
                amendment.NodeId = NodeId;
                amendment.ModifictationDate = DateTime.UtcNow;
                amendment.LastModifiedBy = LastModifiedBy;
                result = await _db.SaveChangesAsync() > 0;
            }
            else
            {
                Node nodeToMouve = amendment.Node;
                nodeToMouve.ParentNodeId = NodeId;
                nodeToMouve.LastModifiedBy = LastModifiedBy;
                nodeToMouve.ModifictationDate = DateTime.UtcNow;
                //_db.Entry(nodeToMouve).State = EntityState.Modified;
                result = await _db.SaveChangesAsync() > 0;
                //amendment.OldNodeId = OldAmdNodeId;
                //amendment.NodeId = NodeId;
                //amendment.ModifictationDate = DateTime.UtcNow;
                //amendment.LastModifiedBy = LastModifiedBy;
                //result = await UpdateAsync(amendment);
                //todo
            }
            return new ServerResponse(result, result ? "Reassignment success" : "Reassignment failed");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(ReassignmentAmendment), nameof(AmendmnetRepository));
            throw;
        }
    }
    public async Task<ServerResponse> SetAmendmentsNumbers(List<Guid> TeamIds, Guid LawId, Guid PhaseLawId, Guid LastModifiedBy)
    {
        try
        {
            var node = await _db.Nodes.FirstOrDefaultAsync(nd => nd.LawId == LawId && nd.PhaseLawId == PhaseLawId && nd.ParentNodeId == null);
            var amendments = await GetAmendmentsListAsync(TeamIds, node.Id);
            int number = 1;
            List<Amendment> amendmentsToSetNumber = new();
            foreach (var amendment in amendments)
            {
                var amend = await findAsync(ad => ad.Id == amendment.Id);
                if (amend != null)
                {
                    amend.Number = number;
                    amendmentsToSetNumber.Add(amend);
                    amend.ModifictationDate = DateTime.UtcNow;
                    amend.LastModifiedBy = LastModifiedBy;
                    await UpdateAsync(amend);
                    number++;
                }
            }
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(SetAmendmentsNumbers), nameof(AmendmnetRepository));
            return new ServerResponse(false, "Fiald");
        }
    }
    public async Task<List<Amendment>> GetAmendmentsForVoteFileCommision(Guid LawId, Guid PhaseLawId, bool includeAmendmentRatraper)
    {
        try
        {
            List<Amendment> amendments = new();
            if (includeAmendmentRatraper)
            {
                amendments = await _db.Amendments
               .AsNoTracking()
               .AsSingleQuery()
               .Where(a => a.Node.PhaseLawId == PhaseLawId && a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString() && !a.IsDeleted && (bool)a.IsAmendementRattrape)
               .Include(a => a.VoteAmendementResult)
               .Include(a => a.Node)
               .Include(a => a.Team)
               .ToListAsync();
            }
            else
            {
                var node = await _db.Nodes
                    .FirstOrDefaultAsync(node => node.PhaseLawId.Equals(PhaseLawId) &&
                    node.LawId.Equals(LawId) &&
                    node.ParentNodeId == null && !node.IsDeleted);
                amendments = await GetAmendments(node.Id);
            }


            return amendments;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsForVoteFileCommision), nameof(AmendmnetRepository));
            throw;
        }
    }

    public async Task<List<StatisticAmendment>> Statistic(Guid Lawid, Guid PhaseLawId)
    {
        try
        {
            var amendments = await _db.Amendments
                .AsNoTracking()
                .AsSingleQuery()
                .Where(a => a.Node.LawId == Lawid && a.Node.PhaseLawId == PhaseLawId && a.AmendmentsStatus == AmendmentsStatus.PUBLIC.ToString() && !a.IsDeleted)
                .Include(a => a.VoteAmendementResult)
                .Include(a => a.Node)
                .Include(a => a.Team)
                .ToListAsync();

            List<StatisticAmendment> statistics = new List<StatisticAmendment>();

            var teams = await _db.Teams
                .AsNoTracking()
                .AsSingleQuery()
                .Where(t => !t.IsDeleted)
                .OrderBy(tm => tm.Ordre).ToListAsync();

            foreach (var team in teams)
            {
                StatisticAmendment statistic = new();
                if (amendments.Count(amd => amd.TeamId == team.Id) > 0)
                {
                    statistic.TeamName = team.Name;
                    statistic.Accepted = amendments
                           .Count(amd => amd.TeamId == team.Id && amd.VoteAmendementResult != null &&
                           (amd.VoteAmendementResult.Result.Equals(VoteResults.VALID.ToString()) ||
                           amd.VoteAmendementResult.Result.Equals(VoteResults.UNANIMOUS.ToString()) ||
                           amd.VoteAmendementResult.Result.Equals(VoteResults.PARTIAL.ToString()) ||
                           amd.VoteAmendementResult.Result.Equals(VoteResults.CONSENSUS.ToString())));
                    statistic.Rejected = amendments
                        .Count(amd => amd.TeamId == team.Id &&
                        (amd.VoteAmendementResult != null && amd.VoteAmendementResult.Result.Equals(VoteResults.REJECT.ToString())));
                    statistic.Suppressed = amendments
                        .Count(amd => amd.TeamId == team.Id &&
                        (amd.VoteAmendementResult != null && amd.VoteAmendementResult.Result.Equals(VoteResults.SUPPRESSED.ToString())));
                    statistic.Totale = amendments.Count(amd => amd.TeamId == team.Id);
                    statistics.Add(statistic);
                }
            }


            return statistics;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetAmendmentsForVoteFileCommision), nameof(AmendmnetRepository));
            throw;
        }
    }

    public async Task<List<VoteResultDto>> GetVoteResult(Guid Lawid, Guid PhaseLawId)
    {
        try
        {
            var subNode = await _db.Nodes.FirstOrDefaultAsync(nd => nd.LawId == Lawid && nd.PhaseLawId == PhaseLawId && !nd.IsDeleted && nd.ParentNode == null);
            var amendments = await GetAmendments(subNode.Id);

            List<VoteResultDto> voteResults = new List<VoteResultDto>();
            foreach (var amendment in amendments)
            {
                var node = await _db.Nodes
                    .Include(nd => nd.ParentNode)
                    .FirstOrDefaultAsync(nd => nd.Id == amendment.Node.ParentNodeId);
                VoteResultDto voteResultDto = new VoteResultDto();
                ParentNode parentNode = new();
                voteResultDto.NodeLabel = GetNodeLabel(amendment.Node);
                voteResultDto.TeamName = amendment.Team.Name;
                voteResultDto.TeamId = amendment.TeamId;
                voteResultDto.Number = amendment.Number;
                voteResultDto.InFavor = amendment.VoteAmendementResult?.InFavor ?? 0;
                voteResultDto.Against = amendment.VoteAmendementResult?.Against ?? 0;
                voteResultDto.Neutral = amendment.VoteAmendementResult?.Neutral ?? 0;
                voteResultDto.Result = amendment.VoteAmendementResult?.Result;
                parentNode.Parent = GetNodeLabel(node.ParentNode);
                parentNode.Child = GetNodeLabel(node);
                voteResultDto.ParentNode = parentNode;
                voteResults.Add(voteResultDto);
            }
            return voteResults;
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetVoteResult), nameof(AmendmnetRepository));
            throw;
        }
    }

    public async Task<ServerResponse> ChangeAmendmentTeam(List<Guid> AmendmentIds, Guid TeamId, Guid LastModifiedBy)
    {
        try
        {
            foreach (var Id in AmendmentIds)
            {
                var amendment = await GetByIdAsync(Id);
                if (amendment is Amendment)
                {
                    amendment.TeamId = TeamId;
                    amendment.ModifictationDate = DateTime.Now;
                    amendment.LastModifiedBy = LastModifiedBy;
                    await UpdateAsync(amendment);
                }
            }
            return new ServerResponse(true, "Operation Success");
        }
        catch (Exception ex)
        {
            E.Loi.Helpers.Trace.Trace.Logging(ex, nameof(GetVoteResult), nameof(AmendmnetRepository));
            throw;
        }
    }

    public async Task<ServerResponse> CheckBeforChangeAmendmentNode(Guid NodeId, Guid AmendmentId)
    {
        try
        {
            var amendment = await _dbSet.AsNoTracking()
                .AsSingleQuery()
                .Include(amd => amd.Node)
                .Include(amd => amd.Node.Type)
                .FirstOrDefaultAsync(amd => amd.Id == AmendmentId);
            if (amendment?.AmendmentIntent != AmendmentIntents.ADDITION.ToString())
            {
                return new ServerResponse(false, "");
            }
            var node = await _db.Nodes
                .AsNoTracking()
                .AsSplitQuery()
                .Include(node => node.Type)
                .FirstOrDefaultAsync(nd => nd.Id == NodeId);
            if (node.Type.IsAmendableAdd)
            {
                var nodeTypes = await getNodeType(NodeId);
                if (nodeTypes.Childrens.Any(ch => ch.Id == amendment.Node.TypeId))
                {
                    return new ServerResponse(false, "");
                }
                return new ServerResponse(true, "");
            }
            return new ServerResponse(true, "");
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task<NodeTypeVm> getNodeType(Guid nodeId)
    {
        var node = await _db.Nodes.FirstOrDefaultAsync(nd => nd.Id == nodeId);
        var nodeType = await _db.NodeTypes.Include(type => type.Children).FirstOrDefaultAsync(type => type.Id == node.TypeId);
        if (nodeType is null)
            return new NodeTypeVm();
        NodeTypeVm nodeTypeVm = new();
        nodeTypeVm.Id = nodeType.Id;
        nodeTypeVm.Label = nodeType.Label;
        nodeTypeVm.Childrens = nodeType.Children.Where(type => type.ContentType == "html").Select(type => new NodeTypeVmChildren { Id = type.Id, Label = type.Label }).ToList();
        return nodeTypeVm;
    }

    public async Task<List<Amendment>> GetNodeAmendmentsByNodeId(Guid NodeId)
    {
        var amendments = await _dbSet
                               .AsNoTracking()
                               .AsSplitQuery()
                               .Include(t => t.Team)
                               .Include(amd => amd.VoteAmendementResult)
                               .OrderBy(t => t.Team.Ordre)
                               .Where(amd => amd.NodeId == NodeId)
                               .ToListAsync();

        return amendments;
    }
}

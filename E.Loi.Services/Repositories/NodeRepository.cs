using E.Loi.DataAccess.Dtos;
using E.Loi.DataAccess.Models;
using E.Loi.Helpers.Enumarations;

namespace E.Loi.Services.Repositories;
public class NodeRepository : BaseRepository<Node>, INodeRepository
{
    List<FlatNode> parentNodes = new();
    private readonly IAmendmnetRepository amendmnetRepository;
    public NodeRepository(ILogger logger, LawDbContext db) : base(logger, db) { }
    public async Task<ServerResponse> CloneNodes(Guid LawId, Guid CurentPhase, Guid DestinationPhase, string Statu)
    {
        try
        {
            var law = await _db.Laws.Include(l => l.PhaseLawIds).FirstOrDefaultAsync(l => l.Id == LawId);
            var statu = await _db.Statuts.FindAsync(law.StatuId);
            var newStatu = await _db.Statuts.FirstOrDefaultAsync(l => l.Order == (statu!.Order + 1));
            law.StatuId = newStatu!.Id;
            _db.Entry(law).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            var node = await _dbSet.FirstOrDefaultAsync(n => n.LawId == LawId && n.PhaseLawId == CurentPhase && n.ParentNodeId == null);
            var nodes = await getNodeChildrens(null, node.Id);
            var phasesLaw = law.PhaseLawIds.ToList();
            foreach (var phaseLaw in phasesLaw)
            {
                phaseLaw!.Statu = PhaseStatu.CLOSED.ToString();
                _db.Entry(phaseLaw).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }

            await _db.PhaseLawIds.AddAsync(new PhaseLawId()
            {
                LawId = LawId,
                PhaseId = DestinationPhase,
                Statu = Statu
            });
            await _db.SaveChangesAsync();
            var response = await CreateNodesFromNodes(nodes, DestinationPhase, LawId);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} CloneNodes function errors", nameof(NodeRepository));
            throw;
        }

    }
    public async Task<Node> CreateVirtuelNode(CreateNodeVm model)
    {
        try
        {
            var existedNode = await _dbSet.FirstOrDefaultAsync(node =>
                                                              node.ParentNodeId == model.ParentNodeId &&
                                                              node.Number == model.Number &&
                                                              node.TypeId == model.TypeId &&
                                                              node.Bis == model.Bis);
            if (existedNode is not null)
                return existedNode;
            var parentNode = await findAsync(n => n.Id == model.ParentNodeId);
            Node node = setNode(model);
            node.Status = NodeStatus.VIRTUAL.ToString();
            node.Order = parentNode.Order > 0 ? parentNode.Order : int.MaxValue;
            //node.IsDeleted = true;
            await _dbSet.AddAsync(node);
            await _db.SaveChangesAsync();
            await RecalculateOrderOnInsertNode(node);
            return node;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} CreateVirtuelNode function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<Node> CreateNode(CreateNodeVm model)
    {
        try
        {
            var nodes = await getWithOptions(n => n.ParentNodeId == model.ParentNodeId && !n.IsDeleted);
            Node node = setNode(model);
            node.Status = NodeStatus.EDITABLE.ToString();
            //node.Order = model.Order;
            node.Order = nodes.Count + 1;
            await _dbSet.AddAsync(node);
            await _db.SaveChangesAsync();
            return node;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} {nameof(GetDirecteChilds)} function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<ServerResponse> CreateLawNodes(List<TextLaw> texts, Guid LawId, Guid CreatedBy)
    {
        try
        {
            var law = await _db.Laws.FirstOrDefaultAsync(l => l.Id == LawId);
            if (law is null)
            {
                return new ServerResponse(false, $"Law Id : {LawId} Not Found");
            }
            var parentNode = await _dbSet.FirstOrDefaultAsync(nd => nd.LawId == LawId && nd.ParentNodeId == null && !nd.IsDeleted && nd.PhaseLawId == law.PhaseLawId);
            parentNode!.CreatedFrom = texts.FirstOrDefault(txt => txt.IdNoeudParent == null)!.IdNoeud;
            parentNode.LastModifiedBy = CreatedBy;
            parentNode.ModifictationDate = DateTime.UtcNow;
            await UpdateAsync(parentNode);
            foreach (var text in texts)
            {
                var NodeType = await _db.NodeTypes.FirstOrDefaultAsync(type => type.Label == Constants.NodeTypes[text.TypeNoeud]);
                if (text.IdNoeudParent != null)
                {
                    Node node = setNode(text, NodeType!.Id, (Guid)law.PhaseLawId!, CreatedBy, LawId);
                    var node_ = await _dbSet.FirstOrDefaultAsync(n => n.CreatedFrom == text.IdNoeudParent);
                    node.ParentNodeId = node_?.Id;
                    await CreateAsync(node);
                }
            }
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} {nameof(CreateLawNodes)} function errors", nameof(NodeRepository));
            throw;
        }

    }
    public async Task<NodeVoteVm[]> GetFlatNodes(Node pnode, Guid? IdLaw, Guid? PhaseLawId)
    {
        try
        {
            List<Node> nodes = new();
            List<NodeVoteVm> nodesVm = new();
            if (pnode == null)
            {
                nodes = await _dbSet
                                     .Include(v => v.VoteNodeResult)
                                     .Include(n => n.Law)
                                     .AsNoTracking()
                                     .AsSplitQuery()
                                     .Where(n => n.LawId == IdLaw && n.PhaseLawId == PhaseLawId && n.ParentNodeId == null && !n.IsDeleted && n.Status != NodeStatus.VIRTUAL.ToString())
                                     .OrderBy(n => n.Order)
                                     .ToListAsync();
            }
            else
            {
                nodes = await _dbSet
                                     .Include(v => v.VoteNodeResult)
                                     .Include(n => n.Law)
                                     .AsNoTracking()
                                     .AsSplitQuery()
                                     .Where(node => node.ParentNodeId == pnode.Id && !node.IsDeleted && node.Status != NodeStatus.VIRTUAL.ToString())
                                     .OrderBy(n => n.Order)
                                     .ToListAsync();
            }
            foreach (var node in nodes)
            {
                nodesVm.AddRange(await GetFlatNodes(node, null, null));
            }
            if (pnode is not null)
            {
                nodesVm.Add(await SetNodeVote(pnode, pnode.Law.Category == "PLF" ? true : false));
            }
            return nodesVm.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetNodeContent function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<NodeContentVm> GetNodeContent(Guid NodeId)
    {
        try
        {
            var node = await _dbSet.AsNoTracking().AsSplitQuery().Include(node => node.Type).FirstOrDefaultAsync(n => n.Id == NodeId);
            var nodeContent = new NodeContentVm();
            nodeContent.Id = NodeId;
            nodeContent.NodeTypeId = node!.TypeId;
            if (ValidateJsonString(node.Content))
            {
                nodeContent.NodeContent = (node.Type.ContentType == "html" || node.Type.ContentType == "ebudgetTable") ? node.Content : generateHtmlFromJsonDtat(ReadJsonString(node.Content));
            }
            else
            {
                nodeContent.NodeContent = node.Content;
            }
            nodeContent.Label = await GetNodeLabel(node);
            return nodeContent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetNodeContent function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<NodeVm[]> GetNodes(Guid IdLaw, Guid PhaseId, bool includeVirtuelNode)
    {
        var law = await _db.Laws.FirstOrDefaultAsync(l => l.Id == IdLaw);
        bool IsPlfLaw = (law?.Category == "PLF" || law?.Category == "CGI" || law?.Category == "Douane") ? true : false;
        var nodes = await GetRecursiveChildren(null!, IdLaw, PhaseId, IsPlfLaw, includeVirtuelNode);
        return nodes;
    }
    private async Task<NodeVm[]> GetRecursiveChildren(Node pnode, Guid? IdLaw, Guid? PhaseLawId, bool IsPlf, bool includeVirtuelNode)
    {
        try
        {
            Node[] nodes;
            if (pnode == null)
            {
                if (includeVirtuelNode)
                {
                    nodes = await _dbSet
                      .AsNoTracking()
                      .AsSplitQuery()
                      .Include(n => n.Law)
                      .Where(n => n.LawId == IdLaw && n.PhaseLawId == PhaseLawId && n.ParentNodeId == null && !n.IsDeleted)
                      .OrderBy(n => n.Order)
                      .ToArrayAsync();
                }
                else
                {
                    nodes = await _dbSet
                      .AsNoTracking()
                      .AsSplitQuery()
                      .Include(n => n.Law)
                      .Where(n => n.LawId == IdLaw && n.PhaseLawId == PhaseLawId && n.Status != NodeStatus.VIRTUAL.ToString() && n.ParentNodeId == null && !n.IsDeleted)
                      .OrderBy(n => n.Order)
                      .ToArrayAsync();
                }

            }
            else
            {
                if (includeVirtuelNode)
                {
                    nodes = await _dbSet
                       .AsNoTracking()
                       .AsSplitQuery()
                       .Include(n => n.Law)
                       .Where(n => n.ParentNodeId == pnode.Id && n.PhaseLawId == PhaseLawId && !n.IsDeleted)
                       .OrderBy(n => n.Order)
                       .ToArrayAsync();
                }
                else
                {

                    nodes = await _dbSet
                        .AsNoTracking()
                        .AsSplitQuery()
                        .Include(n => n.Law)
                        .Where(n => n.ParentNodeId == pnode.Id && n.Status != NodeStatus.VIRTUAL.ToString() && n.PhaseLawId == PhaseLawId && !n.IsDeleted)
                        .OrderBy(n => n.Order)
                        .ToArrayAsync();
                }
            }
            List<NodeVm> nodesVm = new List<NodeVm>();
            if (nodes.Length == 0)
            {
                return nodesVm.ToArray();
            }
            foreach (var node in nodes)
            {
                NodeVm[] getChildren = await GetRecursiveChildren(node, null, PhaseLawId, IsPlf, includeVirtuelNode);
                nodesVm.Add(new NodeVm
                {
                    Id = node.Id,
                    Label = IsPlf ? await GetNodeLabel(node) : string.IsNullOrWhiteSpace(node.Label) ? await GetNodeLabel(node) : node.Label,
                    ParentId = node.ParentNodeId,
                    TypeId = node.TypeId,
                    childrens = getChildren.ToList(),
                    HasColor = await hasAmendment(node.Id),
                    HasNewContent = await hasNewContent(node.Id),
                    IsPlf = IsPlf
                });
            }
            return nodesVm.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetRecursiveChildren function errors", nameof(NodeRepository));
            throw;
        }
    }
    private async Task<NodeVoteVm> SetNodeVote(Node node, bool isPlf)
    {
        _logger.LogInformation(node.Id.ToString());
        return new()
        {
            Id = node.Id,
            Label = isPlf ? await GetNodeLabel(node) : node.Label,
            InFavor = node.VoteNodeResult?.InFavor,
            Against = node.VoteNodeResult?.Against,
            Neutral = node.VoteNodeResult?.Neutral,
            Result = Constants.GetVoteResult(node.VoteNodeResult?.Result ?? string.Empty),
            ResultFr = node.VoteNodeResult?.Result ?? string.Empty,
            VoteId = node.VoteNodeResult?.Id ?? Guid.Empty,
            VoteDate = node.VoteNodeResult?.CreationDate,
            Observation = node.VoteNodeResult?.Observation,
            IdFinance = node.IdFinance ?? Guid.Empty,
        };
    }
    protected async Task<bool> hasAmendment(Guid nodeid)
    {
        var amendments = await _db.Amendments
            .AsNoTracking()
            .AsSingleQuery()
            .Where(a => a.NodeId == nodeid
                        && a.VoteAmendementResult != null
                        && a.VoteAmendementResult!.Result != VoteResults.REJECT.ToString()
                        && a.VoteAmendementResult.Result != VoteResults.SUPPRESSED.ToString())
            .ToListAsync();

        return amendments.Count > 0;
    }
    protected async Task<bool> hasNewContent(Guid nodeid)
    {
        var amendments = await _db.Amendments
            .AsNoTracking()
            .AsSingleQuery()
            .Where(a => a.NodeId == nodeid
                        && !a.IsDeleted && a.AmendmentContents.Count > 0
                        && a.VoteAmendementResult != null
                        && a.VoteAmendementResult!.Result != VoteResults.REJECT.ToString()
                        && a.VoteAmendementResult.Result != VoteResults.SUPPRESSED.ToString())
            .ToListAsync();

        return amendments.Count > 0;
    }
    public async Task<string> GetNodeLabel(Node node)
    {
        var nodeType = await _db.NodeTypes.FirstOrDefaultAsync(t => t.Id == node.TypeId);
        return Constants.GetNodelabel(nodeType!.Label, node.Number, node.Label, node.Bis);
    }
    public async Task<FlatNode[]> GetFlatParents(Guid? nodeId, Node pnode)
    {
        try
        {
            List<Node> nodes = new();
            nodes = await _dbSet
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(n => n.Law)
                    .Where(node => node.Id == nodeId)
                    .ToListAsync();
            foreach (var node in nodes)
            {
                var nodes_ = await GetFlatParents(node.ParentNodeId, node);
                parentNodes.AddRange(nodes_);
            }
            if (pnode != null)
            {
                parentNodes.Add(new FlatNode { Id = pnode.Id, Label = pnode.Law?.Category == "PLF" ? await GetNodeLabel(pnode) : pnode.Label });
            }
            return parentNodes.Distinct().ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetFlatParent function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<List<NodeCounter>> GetDirecteChilds(Guid ParentId)
    {
        try
        {
            var nodes = await getWithOptions(node => node.ParentNodeId == ParentId && !node.IsDeleted);
            return nodes.Select(n => new NodeCounter { Bis = n.Bis, Order = n.Order }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} {nameof(GetDirecteChilds)} function errors", nameof(NodeRepository));
            throw;
        }
    }
    private Node setNode(CreateNodeVm model)
    {
        Node node = new();
        node.Label = model.Label!;
        node.TypeId = model.TypeId ?? Guid.Empty;
        node.Content = model.Content!;
        node.OriginalContent = model.OriginalContent!;
        node.Counter = 0;//TODO
        node.PhaseLawId = model.PhaseLawId ?? Guid.Empty;
        node.ParentNodeId = model.ParentNodeId;
        node.LawId = model.LawId;
        node.Checked = false;
        node.Number = model.Number;
        node.Bis = model.Bis;
        node.IsDeleted = false;
        node.CreatedBy = model.CreatedBy;
        node.CreationDate = DateTime.UtcNow;
        return node;
    }
    private Node setNode(TextLaw text, Guid IdType, Guid PhaseLawId, Guid CreatedBy, Guid LawId)
    {
        Node node = new();
        node.Label = text.TitreAr;
        node.TypeId = IdType;
        node.Content = text.ContenuAr;
        node.OriginalContent = text.ContenuAr;
        node.Counter = 0;//TODO
        node.PhaseLawId = PhaseLawId;
        node.CreatedFrom = text.IdNoeud;
        node.LawId = LawId;
        node.Checked = false;
        node.Number = (int)text.NumeroArticle!;
        node.Bis = 0;//TODO
        node.Order = text.OrdreGenerale;
        node.IsDeleted = false;
        node.CreatedBy = CreatedBy;
        node.Status = NodeStatus.EDITABLE.ToString();
        node.CreationDate = DateTime.UtcNow;
        return node;
    }
    public async Task<ServerResponse> DeleteNode(Guid nodeId, Guid userId)
    {
        try
        {
            var node = await findAsync(n => n.Id == nodeId);
            if (node == null)
                return new ServerResponse(false, $"Node where id {nodeId} not found");
            node.IsDeleted = true;
            node.LastModifiedBy = userId;
            node.ModifictationDate = DateTime.UtcNow;
            return new ServerResponse(await UpdateAsync(node), "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} {nameof(DeleteNode)} function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<ServerResponse> UpdateNodeContent(NodeContentVm nodeContent, Guid UserId)
    {
        try
        {
            var node = await _dbSet.Include(n => n.Law).FirstOrDefaultAsync(n => n.Id == nodeContent.Id);
            if (node is null)
            {
                return new ServerResponse(false, $"Node Where Id :{nodeContent.Id} Is Not Found");
            }
            node.Label = nodeContent.Label;
            node.Content = nodeContent.NodeContent;
            if (node.Law.Category != "PLF")
            {
                node.TypeId = nodeContent.NodeTypeId;
            }
            node.IdFinance = nodeContent.IdFinace;
            node.LastModifiedBy = UserId;
            node.ModifictationDate = DateTime.UtcNow;
            node.Bis = nodeContent.Bis;
            node.Number = nodeContent.Number;
            if (nodeContent.NodeTypeId != Guid.Empty)
                node.TypeId = nodeContent.NodeTypeId;
            var result = await UpdateAsync(node);
            return new ServerResponse(true, "Node Updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(UpdateNodeContent)}", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<ServerResponse> SetPhaseNodes(Guid LawId, Guid DestinationPhase, Guid LastModifiedBy, int Order)
    {
        try
        {
            var law = await _db.Laws.FirstOrDefaultAsync(l => l.Id == LawId);
            if (law is null)
            {
                return new ServerResponse(false, $"Law where Id:{LawId} not found");
            }
            if (Order == 1)
            {
                law!.DateAffectationBureau = DateTime.UtcNow;
            }
            else
            {
                law!.DateAffectationBureau2 = DateTime.UtcNow;
            }
            _db.Update(law);
            await _db.SaveChangesAsync();
            var statu = await _db.Statuts.FindAsync(law.StatuId);
            var newStatu = await _db.Statuts.FirstOrDefaultAsync(s => s.Order == (statu!.Order + 1));
            //   law.PhaseLawId = DestinationPhase;
            law.LastModifiedBy = LastModifiedBy;
            law.StatuId = newStatu!.Id;
            law.ModifictationDate = DateTime.UtcNow;
            _db.Entry(law).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            var phaseLawId = _db.PhaseLawIds.FirstOrDefault(pl => pl.LawId == LawId && pl.Statu == PhaseStatu.OPENED.ToString());
            if (phaseLawId != null)
            {
                phaseLawId.Statu = PhaseStatu.CLOSED.ToString();
                _db.Entry(phaseLawId).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            await _db.PhaseLawIds.AddAsync(new PhaseLawId() { LawId = LawId, PhaseId = DestinationPhase, Statu = PhaseStatu.OPENED.ToString() });
            await _db.SaveChangesAsync();
            var nodes = await getWithOptions(n => n.LawId == law.Id && n.PhaseLawId == phaseLawId!.PhaseId);
            foreach (var node in nodes)
            {
                var node_ = await findAsync(n => n.Id == node.Id);
                node_.PhaseLawId = DestinationPhase;
                node_.LastModifiedBy = LastModifiedBy;
                node_.ModifictationDate = DateTime.UtcNow;
                await UpdateAsync(node_);
            }
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${SetPhaseNodes}", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<Node_VM> GetNode(Node node)
    {
        try
        {
            NodeTitleVM? parentNodeVM = null;
            if (node.ParentNode != null)
            {
                parentNodeVM = await GetParent(node);
            }
            NodeTitleVM[]? childrenNodeVM = await GetDirectChildren(node);
            Node_VM nodeVM = new Node_VM
            {
                Id = node.Id.ToString(),
                Label = node.Label,
                Type = node.TypeId.ToString(),
                status = node.Status.ToString(),
                number = node.Number,
                nature = node.Nature?.ToString() + "",
                PhaseLawId = node.PhaseLawId.ToString(),
                parent = parentNodeVM,
                LawId = node.LawId.ToString(),
                PresentationNote = node.PresentationNote ?? string.Empty,///TODO
                Content = node.Content,
                OriginalContent = node.OriginalContent,
                bis = node.Bis,
                order = node.Order,
                businessRef = node.BusinessRef?.ToString() + "",
                children = childrenNodeVM
            };
            return nodeVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${GetNode}", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<NodeTitleVM> GetParent(Node node)
    {
        try
        {
            NodeTitleVM nodeVM = new NodeTitleVM();
            if (node.ParentNode is not null)
            {
                Node parent = node.ParentNode;
                nodeVM.Id = parent.Id.ToString();
                nodeVM.Label = parent.Label;
                nodeVM.Type = parent.TypeId.ToString();
                nodeVM.status = parent.Status.ToString();
                nodeVM.number = parent.Number;
                nodeVM.bis = parent.Bis;
                nodeVM.order = parent.Order;
                nodeVM.parentNode = parent.ParentNode?.ToString();
                nodeVM.parent = parent.ParentNode is not null ? await GetParent(parent) : null;
            }
            return nodeVM;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${GetParent}", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<NodeTitleVM[]> GetDirectChildren(Node pnode)
    {
        try
        {
            Node[] nodes = await _db.Nodes.Include(n => n.Type).Where(n => (n.ParentNodeId == pnode.Id) && (n.IsDeleted == false)).OrderBy(n => n.Order).ToArrayAsync();

            if (nodes.Length == 0)
            {
                return Array.Empty<NodeTitleVM>();
            }
            else
            {
                List<NodeTitleVM> nodesList = new List<NodeTitleVM>();
                foreach (Node node in nodes)
                {
                    string getLabel = node.Label;
                    string typeLabel = node.Type.Label;
                    nodesList.Add(new NodeTitleVM
                    {
                        Id = node.Id.ToString(),
                        Label = getLabel,
                        Type = node.Type.Id.ToString(),
                        status = node.Status.ToString(),
                        number = node.Number,
                        bis = node.Bis,
                        order = node.Order,
                        parentNode = node?.ParentNode == null ? null : node.ParentNode.ToString(),
                        children = Array.Empty<NodeTitleVM>(),
                    });
                }
                return nodesList.ToArray();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${GetDirectChildren}", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<NodeLawVM[]> GetNodesLawVM(Node node)
    {
        try
        {
            Node[] nodes;
            nodes = await _dbSet
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(n => n.Type)
                    .Where(n => (n.ParentNodeId == node.Id) && (!n.IsDeleted))
                    .OrderBy(n => n.Order)
                    .ToArrayAsync();
            List<NodeLawVM> nodesVm = new List<NodeLawVM>();
            if (nodes.Length == 0)
            {
                return nodesVm.ToArray();
            }
            foreach (var _node in nodes)
            {
                NodeLawVM[] getChildren = await GetNodesLawVM(_node);
                nodesVm.Add(new NodeLawVM
                {
                    id = _node.Id.ToString(),
                    label = node.Label,
                    labelPrint = string.Empty,
                    type = _node.TypeId.ToString(),
                    typeLabel = _node.Type.Label,
                    status = _node.Status,
                    nature = _node.Nature,
                    number = _node.Number,
                    order = _node.Order,
                    bis = _node.Bis,
                    parentNode = _node.ParentNodeId.ToString(),
                    children = getChildren,
                });
            }
            return nodesVm.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetRecursiveChildren function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<List<FlatNode>> GetFlatPrint(Guid LawId, Guid PhaseLawId, Node pNode)
    {
        try
        {
            List<Node> nodes = new();
            List<FlatNode> nodesVm = new();
            if (pNode == null)
            {
                nodes = await _dbSet
                        .AsNoTracking()
                        .Where(n => (n.LawId == LawId) && (n.PhaseLawId == PhaseLawId) && (n.ParentNodeId == null) && (!n.IsDeleted))
                        .OrderBy(n => n.Order)
                        .ToListAsync();
            }
            else
            {
                nodes = await _dbSet
                        .AsNoTracking()
                        .Where(node => (node.ParentNodeId == pNode.Id) && (!node.IsDeleted))
                        .OrderBy(n => n.Order)
                        .ToListAsync();
            }
            if (pNode is not null)
                nodesVm.Add(new FlatNode { Id = pNode.Id, Label = pNode.Label, Content = pNode.Content, Order = pNode.Order });
            foreach (var node in nodes)
            {
                nodesVm.AddRange((await GetFlatPrint(LawId, PhaseLawId, node)).Select(n => new FlatNode { Id = n.Id, Label = n.Label, Content = n.Content, Order = pNode?.Order }));
            }
            return nodesVm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetNodeContent function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<ServerResponse> CreateNodesLawAsync(DataAccess.Dtos.NodeDto[] nodesDto, Guid LawId)
    {
        try
        {
            foreach (var nodeDto in nodesDto)
            {
                var law = await _db.Laws
                          .AsNoTracking()
                          .AsSingleQuery()
                          .Include(l => l.PhaseLawIds)
                          .Include(l => l.Statu)
                          .FirstOrDefaultAsync(l => l.IdFinance == LawId);
                Node newNode = new();
                newNode.Label = nodeDto.Label is null ? string.Empty : nodeDto.Label;
                newNode.TypeId = nodeDto.TypeId;
                newNode.Content = nodeDto.Content is null ? string.Empty : nodeDto.Content;
                newNode.OriginalContent = nodeDto.OriginalContent is null ? string.Empty : nodeDto.OriginalContent;
                newNode.PresentationNote = nodeDto.PresentationNote is null ? string.Empty : nodeDto.PresentationNote;
                newNode.Counter = 0;//TODO
                if (law.Statu != null && law.Statu.Order == 6)
                {
                    newNode.PhaseLawId = Guid.Parse("2E98A95D-7D69-4C4F-B277-08DB11F5E309");
                }
                else
                {
                    newNode.PhaseLawId = law?.PhaseLawIds.FirstOrDefault(p => p.Statu == PhaseStatu.OPENED.ToString())!.PhaseId ?? Guid.Empty;
                }
                newNode.LawId = law!.Id;
                newNode.Checked = false;
                newNode.Number = nodeDto.Number;
                newNode.Bis = nodeDto.Bis;
                newNode.Order = nodeDto.Order;
                newNode.IsDeleted = false;
                newNode.CreatedBy = Guid.Parse("F37BE3F3-9E2E-4E64-90EF-DCF48F38BF48");
                newNode.CreationDate = DateTime.UtcNow;
                newNode.IdFinance = nodeDto.Id;
                newNode.Status = nodeDto.Status.ToString();
                newNode.Nature = nodeDto.Nature.ToString();
                if (nodeDto.ParentNodeId != null)
                {
                    var node_ = await _dbSet.FirstOrDefaultAsync(n => n.IdFinance == nodeDto.ParentNodeId);
                    newNode.ParentNodeId = node_?.Id;
                }
                await CreateAsync(newNode);
                await CreateNodesLawAsync(nodeDto.Childrens, LawId);
            }
            return new ServerResponse(true, "Success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} CreateLawNodes function errors", nameof(NodeRepository));
            return new ServerResponse(false, "Fail");
        }
    }
    public Root ReadJsonString(string stringJson)
    {
        Root root = new();
        root = System.Text.Json.JsonSerializer.Deserialize<Root>(stringJson)!;
        return root;
    }
    public string generateHtmlFromJsonDtat(Root root)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("<style> .json-table, th, td {border: 1px solid #ddd;border-collapse: collapse;}table th{background-color: #ececec}</style><table class='json-table'>");
        stringBuilder.Append("<tr>");
        stringBuilder.Append("<th colspan='5' style='max-width:20%;vertical-align:middle;padding:10px text-align:center !important;'></th>");
        stringBuilder.Append("<th style='width:50%;vertical-align:middle;text-align:center !important;padding:10px'>اسم</th>");
        stringBuilder.Append("<th style='width:10%;vertical-align:middle;text-align:center !important;padding:10px'>الرسم</th>");
        stringBuilder.Append("<th style='width:10%;vertical-align:middle;text-align:center !important;padding:10px>الوحدة</th>");
        stringBuilder.Append("<th style='width:10%;vertical-align:middle;text-align:center !important;padding:10px'>الوحدة التكميلية</th>");
        stringBuilder.Append("</tr>");
        foreach (var row in root.rows)
        {
            stringBuilder.Append($"<tr><td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.nomenclature_level_1}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.nomenclature_level_2}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.nomenclature_level_3}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.nomenclature_level_4}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.nomenclature_level_5}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.designation}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.tarif}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.unit}</td>");
            stringBuilder.Append($"<td style='vertical-align:middle;text-align:center !important;padding:5px'>{row.unit_complementary}</td></tr>");
        }
        stringBuilder.Append("</table>");
        return stringBuilder.ToString();
    }
    public bool ValidateJsonString(string stringJson)
    {
        try
        {
            JToken.Parse(stringJson);
            return true;
        }
        catch (JsonReaderException)
        {
            return false;
        }
    }
    public async Task<NodeVm> GetParentNode(Guid lawId, Guid phaseLawId)
    {
        try
        {
            var node = await findAsync(node => node.LawId == lawId && node.PhaseLawId == phaseLawId && node.ParentNodeId == null);
            NodeVm nodeVm = new()
            {
                Id = node.Id,
                Label = node.Label,
            };
            return nodeVm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetParentNode function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<FlatNode[]> GetLawSections(Guid Lawid, Guid PhaseLawId, bool IsPrinVotingFile)
    {
        try
        {
            List<Node> nodes = await _dbSet
                                .AsNoTracking()
                                .AsSplitQuery()
                                .Where(n => n.LawId == Lawid && n.PhaseLawId == PhaseLawId && n.Type.Label == "الجزء")
                                .ToListAsync();
            List<FlatNode> sections = new List<FlatNode>();
            foreach (var node in nodes)
            {
                FlatNode flatNode = new();
                var createdFrom = await _dbSet.FirstOrDefaultAsync(n => n.CreatedFrom == node.Id);
                if (createdFrom is null && !IsPrinVotingFile)
                {
                    flatNode.Id = node.Id;
                    flatNode.Label = await GetNodeLabel(node);
                    sections.Add(flatNode);
                }
                if (IsPrinVotingFile)
                {
                    flatNode.Id = node.Id;
                    flatNode.Label = await GetNodeLabel(node);
                    sections.Add(flatNode);
                }
            }
            return sections.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetLawSections function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<ServerResponse> cloneSectionWithChildrens(Guid sectionId, Guid destinationPhase)
    {
        try
        {
            var section = await _dbSet.Include(p => p.ParentNode).FirstOrDefaultAsync(n => n.Id == sectionId);
            var roote = await findAsync(n => n.CreatedFrom == section.ParentNodeId && n.PhaseLawId == destinationPhase);
            if (roote is null)
            {
                var newRoot = new Node();
                newRoot.Label = section?.ParentNode?.Label!;
                newRoot.TypeId = section?.ParentNode?.TypeId ?? Guid.Empty;
                newRoot.Content = section?.ParentNode?.Content!;
                newRoot.OriginalContent = section?.ParentNode?.OriginalContent!;
                newRoot.PresentationNote = section?.ParentNode?.PresentationNote!;
                newRoot.Counter = section?.ParentNode?.Counter ?? 0;
                newRoot.ParentNodeId = null;
                newRoot.PhaseLawId = destinationPhase;
                newRoot.LawId = section!.LawId;
                newRoot.Checked = section?.ParentNode?.Checked ?? false;
                newRoot.IsDeleted = false;
                newRoot.Status = section?.ParentNode?.Status!;
                newRoot.Number = section!.ParentNode!.Number;
                newRoot.Order = section.ParentNode.Order;
                newRoot.Bis = section.ParentNode.Bis;
                newRoot.BusinessRef = section?.ParentNode?.BusinessRef;
                newRoot.CreatedFrom = section?.ParentNode.Id;
                newRoot.CreatedBy = section!.ParentNode.CreatedBy;
                newRoot.CreationDate = DateTime.UtcNow;
                newRoot.Nature = section.ParentNode.Nature;
                await CreateAsync(newRoot);
                await _db.PhaseLawIds.AddAsync(new PhaseLawId()
                {
                    LawId = section.LawId,
                    PhaseId = destinationPhase,
                    Statu = PhaseStatu.OPENED.ToString(),
                });
                await _db.SaveChangesAsync();
            }
            var nodes = await getNodeChildrens(null, sectionId);
            var response = await CreateNodesFromNodes(nodes, destinationPhase, section.LawId);
            return response;
        }
        catch (Exception ex)
        {
            return new ServerResponse(false, "Fail");
        }
    }
    private async Task<DataAccess.Dtos.NodeDto[]> getNodeChildrens(Node? pnode, Guid? sectionId)
    {
        try
        {
            Node[] nodes;
            if (pnode == null)
            {
                nodes = await _dbSet
                        .AsNoTracking()
                        .AsSplitQuery()
                        .Include(n => n.Law)
                        .Where(n => n.Id == sectionId && !n.IsDeleted)
                        .OrderBy(n => n.Order)
                        .ToArrayAsync();
            }
            else
            {
                nodes = await _dbSet
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(n => n.Law)
                    .Where(n => n.ParentNodeId == pnode.Id && !n.IsDeleted)
                    .OrderBy(n => n.Order)
                    .ToArrayAsync();
            }
            List<DataAccess.Dtos.NodeDto> nodesDto = new List<DataAccess.Dtos.NodeDto>();
            if (nodes.Length == 0)
            {
                return nodesDto.ToArray();
            }
            foreach (var node in nodes)
            {
                DataAccess.Dtos.NodeDto[] getChildren = await getNodeChildrens(node, sectionId);
                nodesDto.Add(new DataAccess.Dtos.NodeDto
                {
                    Id = node.Id,
                    Label = node.Label,
                    Content = node.Content,
                    OriginalContent = node.OriginalContent,
                    PresentationNote = node.PresentationNote,
                    ParentNodeId = node.ParentNodeId,
                    TypeId = node.TypeId,
                    Status = (NodeStatus)Enum.Parse(typeof(NodeStatus), node.Status),
                    Number = node.Number,
                    Order = node.Order,
                    Bis = node.Bis,
                    Nature = (node.Nature != null && node.Nature != string.Empty) ? (NodeOrigin)Enum.Parse(typeof(NodeOrigin), node.Nature) : null,
                    Childrens = getChildren,
                });
            }
            return nodesDto.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetRecursiveChildren function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<ServerResponse> CreateNodesFromNodes(DataAccess.Dtos.NodeDto[] nodesDto, Guid destinationPhase, Guid LawId)
    {
        try
        {
            foreach (var nodeDto in nodesDto)
            {
                var newNode = new Node();
                newNode.Label = nodeDto.Label!;
                newNode.TypeId = nodeDto.TypeId;
                var amendments = await _db.Amendments
                                               .Include(am => am.VoteAmendementResult)
                                               .Where(am => am.NodeId == nodeDto.Id
                                                && !am.IsDeleted
                                                && (am.VoteAmendementResult!.Result == nameof(VoteResults.VALID)
                                                || am.VoteAmendementResult.Result == nameof(VoteResults.PARTIAL)
                                                || am.VoteAmendementResult.Result == nameof(VoteResults.UNANIMOUS)))
                                                .ToListAsync();
                if (amendments.Count > 0)
                {
                    var amendment = amendments.First();
                    newNode.OriginalContent = amendment.NewContent!;
                    newNode.Content = amendment.NewContent!;
                    newNode.Status = NodeStatus.EDITABLE.ToString();
                }
                else
                {
                    newNode.OriginalContent = nodeDto.OriginalContent!;
                    newNode.Content = nodeDto.Content!;
                }
                newNode.PresentationNote = nodeDto.PresentationNote;
                newNode.PhaseLawId = destinationPhase;
                newNode.LawId = LawId;
                newNode.Number = nodeDto.Number;
                newNode.Bis = nodeDto.Bis;
                newNode.Order = nodeDto.Order;
                newNode.IsDeleted = false;
                newNode.CreationDate = DateTime.UtcNow;
                newNode.CreatedFrom = nodeDto.Id;
                newNode.Nature = nodeDto.Nature.ToString();
                

                if (nodeDto.ParentNodeId != null)
                {
                    var node_ = await _dbSet.FirstOrDefaultAsync(n => n.CreatedFrom == nodeDto.ParentNodeId);
                    newNode.ParentNodeId = node_?.Id;
                }
                var node = await _dbSet.Include(nd => nd.PhaseLaw).FirstOrDefaultAsync(n => n.Id == nodeDto.Id);

                var acceptNode = await _dbSet.FirstOrDefaultAsync(n => n.Id == nodeDto.Id
                           && (n.VoteNodeResult!.Result == nameof(VoteResults.VALID)
                           || n.VoteNodeResult.Result == nameof(VoteResults.PARTIAL)
                           || n.VoteNodeResult.Result == nameof(VoteResults.UNANIMOUS)));

                if (node.PhaseLaw.Order == 4 || node.PhaseLaw.Order == 8)
                {

                    if (acceptNode != null)
                    {
                        newNode.Status = NodeStatus.EDITABLE.ToString();
                        await CreateAsync(newNode);
                        await CreateNodesFromNodes(nodeDto.Childrens, destinationPhase, LawId);
                    }
                }
                else
                {
                    if (node.Status == NodeStatus.VIRTUAL.ToString())
                    {
                        await CreateAsync(newNode);
                        await CreateNodesFromNodes(nodeDto.Childrens, destinationPhase, LawId);
                    }
                    else
                    {
                        if (acceptNode != null)
                        {
                            await CreateAsync(newNode);
                            await CreateNodesFromNodes(nodeDto.Childrens, destinationPhase, LawId);
                        }
                    }
                }
            }
            return new ServerResponse(true, "success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} CloneNodes function errors", nameof(NodeRepository));
            return new ServerResponse(false, "Fail");
        }
    }
    public async Task<Node> GetNodeByID(Guid Id)
    {
        try
        {
            return await _dbSet
                         .AsNoTracking()
                         .AsSingleQuery()
                         .Include(v => v.Type)
                         .Include(v => v.VoteNodeResult)
                         .Include(n => n.ParentNode)
                         .FirstOrDefaultAsync(n => n.Id == Id) ?? new Node();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetNodeByID function errors", nameof(NodeRepository));
            return new Node();
        }
    }
    public async Task<NodeVoteVm[]> GetSectionWithNodes(Guid section, Node pnode)
    {
        try
        {

            List<Node> nodes = new();
            List<NodeVoteVm> nodesVm = new();
            if (pnode == null)
            {
                nodes = await _dbSet
                                     .Include(v => v.VoteNodeResult)
                                     .Include(n => n.Law)
                                     .AsNoTracking()
                                     .AsSplitQuery()
                                     .Where(n => n.Id == section && !n.IsDeleted && n.Status != NodeStatus.VIRTUAL.ToString())
                                     .OrderBy(n => n.Order)
                                     .ToListAsync();
            }
            else
            {
                nodes = await _dbSet
                                     .Include(v => v.VoteNodeResult)
                                     .Include(n => n.Law)
                                     .AsNoTracking()
                                     .AsSplitQuery()
                                     .Where(node => node.ParentNodeId == pnode.Id && !node.IsDeleted && node.Status != NodeStatus.VIRTUAL.ToString())
                                     .OrderBy(n => n.Order)
                                     .ToListAsync();
            }
            foreach (var node in nodes)
            {
                nodesVm.AddRange(await GetFlatNodes(node, null, null));
            }
            if (pnode is not null)
            {
                nodesVm.Add(await SetNodeVote(pnode, pnode.Law.Category == "PLF" ? true : false));
            }
            return nodesVm.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetSectionWithNodes function errors", nameof(NodeRepository));
            throw;
        }
    }
    private async Task RecalculateOrderOnInsertNode(Node creatednode)
    {
        var nodes = await getWithOptions(nd => nd.ParentNodeId == creatednode.ParentNodeId);
        List<Node> nodesToUpdateOrder = new List<Node>();

        var node = nodes.FirstOrDefault(nd => (nd.Number == creatednode.Number || nd.Number == creatednode.Number - 1) && nd.Status != NodeStatus.VIRTUAL.ToString());
        if (node != null)
        {
            if (creatednode.Bis > 0)
            {
                node = nodes.FirstOrDefault(nd => nd.Number == creatednode.Number && nd.Bis == creatednode.Bis - 1);
            }
            nodesToUpdateOrder = nodes.Where(o => o.Order > node?.Order && o.Id != creatednode.Id).ToList();
            creatednode.Order = node.Order + 1;
            await _db.SaveChangesAsync();
            foreach (var nodeToUpdateOrder in nodesToUpdateOrder)
            {
                nodeToUpdateOrder.Order = nodeToUpdateOrder.Order + 1;
                await _db.SaveChangesAsync();
            }
        }
        else
        {
            //nodes.Sort((x, y) => x.Number.CompareTo(y.Number));
            //int order = 1;
            //foreach (var nd in nodes)
            //{
            //    nd.Order = order;
            //    await _db.SaveChangesAsync();
            //    order++;
            //}
            int maxOrder = nodes.Max(nd => nd.Order);
            creatednode.Order = maxOrder + 1;
            await _db.SaveChangesAsync();
        }
    }
    public async Task<SectionDto> GetSectionRubric(Guid sectionId)
    {
        try
        {
            var node = await findAsync(node => node.Id == sectionId);
            if (node is null)
                return null;
            SectionDto section = new SectionDto();
            section.SectionNumber = GetSectionNumber(node.Number);
            section.SectionName = node.Label;

            var nodes = await getWithOptions(n => n.ParentNodeId == node.Id && !n.IsDeleted);
            List<ChapterDto> chapters = new List<ChapterDto>();
            List<ChapterArticleDto> articleList = new List<ChapterArticleDto>();
            List<Node> articles = new();

            foreach (var _node in nodes)
            {
                articles.Clear();
                ChapterDto chapterDto = new ChapterDto();
                chapterDto.Title = GetChapter(_node.Number);
                chapterDto.SubTitle = _node.Label;
                articles = await GetArticles(_node);
                chapterDto.StartArticle = articles.Where(x => x.Type != null && x.Type.Label.Trim() == "المادة").Min(a => a.Number);
                chapterDto.EndArticle = articles.Where(x => x.Type != null && x.Type.Label.Trim() == "المادة").Max(a => a.Number);
                chapters.Add(chapterDto);

                ChapterArticleDto article = new ChapterArticleDto();
                article.Title = GetChapter(_node.Number);
                article.SubTitle = _node.Label;
                article.StartArticle = articles.Where(x => x.Type != null && x.Type.Label.Trim() == "المادة").Min(a => a.Number);
                article.EndArticle = articles.Where(x => x.Type != null && x.Type.Label.Trim() == "المادة").Max(a => a.Number);
                List<ChapterItem> chaptersItem = new List<ChapterItem>();
                foreach (var nodeItem in articles)
                {
                    ChapterItem chapterItem = new ChapterItem();
                    chapterItem.Id = nodeItem.Id;
                    // chapterItem.CreatedFrom = nodeItem.CreatedFrom ?? Guid.Empty;
                    var node_ = await _dbSet
                           .AsNoTracking()
                           .AsSplitQuery()
                           .Include(v => v.VoteNodeResult)
                           .Include(n => n.Law).FirstOrDefaultAsync(x => x.Id == nodeItem.Id);
                    var Childrens = await GetFlatChildes(node_);
                    List<Children> listChildrens = new();
                    foreach (var Children in Childrens)
                    {
                        var amends = await GetNodeAmendmentsByNodeId(Children.Id);
                        listChildrens.Add(new DataAccess.Dtos.Children
                        {
                            Id = Children.Id,
                            Label = Children.Label ?? string.Empty,
                            Amendments = amends,
                        });
                    }


                    // chapterItem.Childrens = Childrens.Where(nd=>nd.Id !=nodeItem.Id).Select(nd=>new Children { Id=nd.Id ,Label=nd.Label}).ToList();
                    chapterItem.Childrens = listChildrens;
                    chapterItem.Label = await GetNodeLabel(nodeItem);
                    var vote = await _db.VoteNodeResults.FirstOrDefaultAsync(v => v.NodePhaseLawId == nodeItem.CreatedFrom);
                    chapterItem.vote = new VoteSessionDto { InFavor = vote?.InFavor, Against = vote?.Against, Neutral = vote?.Neutral, Result = vote?.Result };
                    if (!chapterItem.Label.Contains("الباب"))
                    {
                        chaptersItem.Add(chapterItem);

                    }
                }

                article.chapters = chaptersItem;
                articleList.Add(article);

            }

            section.RubricList = chapters;
            section.ArticleList = articleList;
            return section;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetSectionRubric function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<List<Node>> GetArticles(Node parentNode)
    {
        try
        {
            List<Node> nodesVm = new();
            var nodes = await _dbSet
                     .AsNoTracking()
                     .AsSplitQuery()
                     .Include(node => node.Law)
                     .Include(node => node.Type)
                     .Include(node => node.VoteNodeResult)
                     .Where(node => node.ParentNodeId == parentNode.Id && !node.IsDeleted && node.Status == NodeStatus.EDITABLE.ToString())
                     .OrderBy(n => n.Order)
                     .ToListAsync();

            foreach (var node in nodes)
            {
                //nodesVm.AddRange(await GetArticles(node));
                foreach (var children in await GetArticles(node))
                {
                    if (children.Type.Label.Trim() == "المادة")
                    {
                        nodesVm.Add(children);
                    }
                }
            }
            if (parentNode is not null)
            {
                nodesVm.Add(parentNode);
            }
            return nodesVm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetNodeContent function errors", nameof(NodeRepository));
            throw;
        }
    }
    public static string GetSectionNumber(int Number) => Number switch
    {
        1 => "الجزء الأول",
        2 => "الجزء الثاني",
        _ => string.Empty
    };
    public static string GetChapter(int Number) => Number switch
    {
        1 => "الباب الأول",
        2 => "الباب الثاني",
        3 => "الباب الثالث",
        4 => "الباب الرابع",
        5 => "الباب الخامس",
        6 => "الباب السادس",
        7 => "الباب السابع",
        8 => "الباب الثامن",
        9 => "الباب التاسع",
        10 => "الباب العاشر",
        _ => string.Empty
    };
    public async Task<ServerResponse> SetNewContent(UpdateNodeContent Node)
    {
        try
        {
            var node = await findAsync(nd => nd.Id == Node.NodeId);
            if (node is null)
                return new ServerResponse(false, $"Node where Id:{Node.NodeId} not found");
            node.Content = Node.NewContent;
            node.OldContent = Node.NewContent;
            node.ModifictationDate = DateTime.Now;
            node.LastModifiedBy = Node.LastModifiedBy;
            var result = await UpdateAsync(node);
            return new ServerResponse(result, result ? "Success" : "Field");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetNodeContent function errors", nameof(UpdateNodeContent));
            throw;
        }
    }
    public async Task<List<LawPart>> GetNodeWithAmendmentForPresident(Guid LawId, Guid PhaseId)
    {
        List<LawPart> lawParts = new();
        var parts = await _dbSet
                                .AsNoTracking()
                                .AsSingleQuery()
                                .Where(node =>
                                             node.LawId.Equals(LawId) &&
                                             node.PhaseLawId.Equals(PhaseId) &&
                                             !node.IsDeleted &&
                                             node.Type.Label == "الجزء"
                                )
                                .ToListAsync();


        foreach (var part in parts)
        {

            string label = await GetNodeLabel(part);
            var nodes = await GetArticles(part);
            List<NodePart> nodeParts = new List<NodePart>();
            foreach (var node in nodes)
            {
                var NodeParent = await _dbSet
                    .Include(nd => nd.ParentNode)
                    .Include(nd => nd.ParentNode.ParentNode)
                    .FirstOrDefaultAsync(nd => nd.Id == node.Id);
                var nodePart = new NodePart();
                nodePart.Id = node.Id;
                nodePart.Parent = NodeParent.ParentNode?.ParentNode != null ? await GetNodeLabel(NodeParent.ParentNode.ParentNode) : string.Empty;
                nodePart.SubPrent = NodeParent.ParentNode != null ? (await GetNodeLabel(NodeParent.ParentNode)).Trim() : string.Empty;
                nodePart.Label = (await GetNodeLabel(node)).Trim();
                nodeParts.Add(nodePart);
            }
            lawParts.Add(new LawPart
            {
                Label = label,
                NodesPart = nodeParts
            });
        }
        return lawParts;
    }
    public async Task<NodeVoteVm[]> GetFlatChildes(Node parentNode)
    {
        try
        {
            List<Node> nodes = new();
            List<NodeVoteVm> nodesVm = new();

            nodes = await _dbSet
                                 .AsNoTracking()
                                 .AsSplitQuery()
                                 .Include(v => v.VoteNodeResult)
                                 .Include(n => n.Law)
                                 .Where(node => node.ParentNodeId == parentNode.Id && !node.IsDeleted)
                                 .OrderBy(n => n.Order)
                                 .ToListAsync();
            foreach (var node in nodes)
            {
                nodesVm.AddRange(await GetFlatChildes(node));
            }
            if (parentNode != null)
            {
                nodesVm.Add(await SetNodeVote(parentNode, parentNode.Law.Category == "PLF" ? true : false));
            }
            return nodesVm.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{ex.Message} GetNodeContent function errors", nameof(NodeRepository));
            throw;
        }
    }
    public async Task<List<NodeVoteVm>> GetSectionForVoting(Guid sectionId)
    {
        var node = await _dbSet
            .AsNoTracking()
                                 .AsSplitQuery()
                                 .Include(v => v.VoteNodeResult)
                                 .Include(n => n.Law).FirstOrDefaultAsync(x => x.Id == sectionId);
        return (await GetFlatChildes(node)).ToList();
    }
    public async Task<List<AmendmentSession>> GetNodeAmendmentsByNodeId(Guid NodeId)
    {
        var amendments = await _db.Amendments
                               .AsNoTracking()
                               .AsSplitQuery()
                               .Include(t => t.Team)
                               .Include(amd => amd.VoteAmendementResult)
                               .OrderBy(t => t.Team.Ordre)
                               .Where(amd => amd.NodeId == NodeId)
                               .ToListAsync();

        return amendments.Select(amendment => new AmendmentSession
        {
            TeamId = amendment.TeamId,
            TeamName = amendment.Team.Name,
            Order = amendment.Ordre,
            Number = amendment.Number,
            AmendmentIntent = amendment.AmendmentIntent,
            NumberSystem = amendment.NumSystem,
        }).ToList();
    }
    public async Task<ServerResponse> GetNodeByTypeByNumberByBis(Guid ParentNodeId,Guid TypeId, int Number, int Bis)
    {
        try
        {
            var node = await _dbSet
                .AsNoTracking()
                .AsSingleQuery()
                .FirstOrDefaultAsync(node => node.ParentNodeId==ParentNodeId && node.TypeId == TypeId && node.Number == Number && node.Bis == Bis && node.Status == NodeStatus.EDITABLE.ToString());
            if (node is null)
                return new ServerResponse(false,"Node Not Exist");
            return new ServerResponse(true, "Node Exist");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {nameof(GetNodeByTypeByNumberByBis)}", nameof(NodeRepository));
            throw;
        }
    }
}



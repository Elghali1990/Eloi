namespace E.Loi.Services.Repositories;

public class NodeTypeRepository : BaseRepository<NodeType>, INodeTypeRepository
{
    public NodeTypeRepository(ILogger logger, LawDbContext db) : base(logger, db) { }

    public async Task<List<NodeTypeVm>> GetNodeTypesAsync()
    {
        try
        {
            var nodeTypes = await GetAllAsync();
            return nodeTypes.Select(n => new NodeTypeVm { Id = n.Id, Label = n.Label, Childrens = n.Children.Select(c => new NodeTypeVmChildren { Id = c.Id, Label = c.Label }).ToList() }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error when calling {nameof(GetNodeTypesAsync)}", nameof(NodeTypeRepository));
            throw;
        }
    }

    public async Task<List<NodeTypeVm>> GetPlfNodeTypesWithNodeHierarchies()
    {
        try
        {
            var nodeHierarchyFamillie = await _db.NodeHierarchyFamillies.FirstOrDefaultAsync(famillie => famillie.Label == "أنواع عقد قوانين المالية");
            var nodeTypes = await _dbSet.Include(type => type.Children).Where(type => type.FamillyId == nodeHierarchyFamillie!.Id && type.TextType == null).ToListAsync();
            return nodeTypes.Select(n => new NodeTypeVm { Id = n.Id, Label = n.Label, Childrens = n.Children.Select(c => new NodeTypeVmChildren { Id = c.Id, Label = c.Label }).ToList() }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error when calling {nameof(GetNodeTypesAsync)}", nameof(NodeTypeRepository));
            throw;
        }
    }

    public async Task<NodeTypeVm> GetNodeTypesWithNodeHierarchies(Guid ParentId)
    {
        try
        {
            var nodeType = await _dbSet.Include(type => type.Children).FirstOrDefaultAsync(type => type.Id == ParentId);
            if (nodeType is null)
                return new NodeTypeVm();
            NodeTypeVm nodeTypeVm = new();
            nodeTypeVm.Id = nodeType.Id;
            nodeTypeVm.Label = nodeType.Label;
            nodeTypeVm.Childrens = nodeType.Children.Where(type => type.ContentType == "html").Select(type => new NodeTypeVmChildren { Id = type.Id, Label = type.Label }).ToList();
            return nodeTypeVm;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error when calling {nameof(GetNodeTypesAsync)}", nameof(NodeTypeRepository));
            throw;
        }
    }
    public async Task<ServerResponse> InsertNodeTypesAsync(List<NodeTypeDto> nodeTypesList)
    {
        try
        {
            List<NodeType> nodeTypes = new();
            foreach (var nodeType in nodeTypesList)
            {
                var nodeType_ = await findAsync(nt => nt.Label.Trim() == nodeType.Label.Trim());
                if (nodeType_ is null)
                {
                    nodeTypes.Add(new NodeType
                    {
                        Label = nodeType.Label,
                        ContentType = nodeType.ContentType,
                        IsAmendableAdd = nodeType.IsAmendableAdd,
                        IsAmendableDelete = nodeType.IsAmendableDelete,
                        IsAmendableEdit = nodeType.IsAmendableEdit,
                        IsVotable = nodeType.IsVotable,
                        IdFinance = nodeType.Id,
                        IsDeleted = false
                    });
                }
            }
            if (nodeTypes.Count == 0)
            {
                return new ServerResponse(true, "Node types already exist");
            }
            await _dbSet.AddRangeAsync(nodeTypes);
            var response = await _db.SaveChangesAsync() > 0;
            return new ServerResponse(Flag: response, Massage: response ? "Success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error when calling {nameof(InsertNodeTypesAsync)}", nameof(NodeTypeRepository));
            throw;
        }
    }
}

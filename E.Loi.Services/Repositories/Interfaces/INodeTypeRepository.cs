namespace E.Loi.Services.Repositories.Interfaces;

public interface INodeTypeRepository : IBaseRepository<NodeType>
{
    Task<List<NodeTypeVm>> GetNodeTypesAsync();
    Task<ServerResponse> InsertNodeTypesAsync(List<NodeTypeDto> nodeTypesList);
    Task<List<NodeTypeVm>> GetPlfNodeTypesWithNodeHierarchies();
    Task<NodeTypeVm> GetNodeTypesWithNodeHierarchies(Guid ParentId);
}

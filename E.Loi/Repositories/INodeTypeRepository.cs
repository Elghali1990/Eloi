namespace E.Loi.Repositories;

public interface INodeTypeRepository
{
    Task<NodeType> GetNodeTypeByIdAsync(Guid nodeId);
    Task<NodeType> GetNodeTypeByNameAsync(string label);
    Task<List<NodeTypeVm>> GetNodeTypesAsync();
    Task<NodeTypeVm> GetNodeTypesWithNodeHierarchies(Guid ParentId);
}

namespace E.Loi.DataAccess.Vm.Node;

public class NodeVm
{
    public Guid Id { get; set; }
    public string? Label { get; set; }
    public Guid? ParentId { get; set; }
    public Guid TypeId { get; set; }
    public bool IsPlf { get; set; }
    public List<NodeVm>? childrens { get; set; }
    public bool IsCollapsed { get; set; } = false;
    public bool IsSelected { get; set; } = false;
    public bool HasChildren => childrens != null && childrens.Any();
    public bool HasColor { get; set; } = false;
    public bool HasNewContent { get; set; } = false;
    public bool HasMatchingChild { get; set; } = false;
    public bool MatchesSearch { get; set; } = true;

}
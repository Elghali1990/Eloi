namespace E.Loi.DataAccess.Vm.NodeType;

public class NodeTypeVm
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public List<NodeTypeVmChildren> Childrens { get; set; } = new();

}

public class NodeTypeVmChildren
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;

}


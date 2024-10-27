namespace E.Loi.DataAccess.Vm.Node;

public class NodeDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = default!;
    public Guid TypeId { get; set; }
    public string Content { get; set; } = default!;
    public string OriginalContent { get; set; } = default!;
    public string PresentationNote { get; set; } = default!;
    public Guid? ParentNodeId { get; set; }
    public string Status { get; set; } = default!;
    public int Number { get; set; } = default!;
    public int Bis { get; set; } = default!;
    public int Order { get; set; } = default!;
    public string Nature { get; set; } = default!;
}

namespace E.Loi.DataAccess.Vm.Node;
public class Node_VM
{
    public string Id { get; set; } = default!;
    public string Label { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string OriginalContent { get; set; } = default!;
    public string PresentationNote { get; set; } = default!;
    public string PhaseLawId { get; set; } = default!;
    public string nature { get; set; } = default!;

    public NodeTitleVM? parent { get; set; }
    public string LawId { get; set; } = default!;
    public string status { get; set; } = default!;
    public int number { get; set; }
    public int bis { get; set; }
    public int order { get; set; }
    public string businessRef { get; set; } = default!;
    public NodeTitleVM[] children { get; set; }
}
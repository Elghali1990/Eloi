namespace E.Loi.DataAccess.Vm.Node;
public class CreateNodeVm
{
    public string? Label { get; set; }
    public Guid? TypeId { get; set; }
    public string? Content { get; set; }
    public string? OriginalContent { get; set; }
    public string? PresentationNote { get; set; }
    public Guid? PhaseLawId { get; set; }
    public Guid? ParentNodeId { get; set; }
    public Guid LawId { get; set; }
    public int Number { get; set; }
    public int Bis { get; set; }
    public int Order { get; set; }
    public Guid CreatedBy { get; set; }
}

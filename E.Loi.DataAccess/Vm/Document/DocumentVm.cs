namespace E.Loi.DataAccess.Vm.Document;

public class DocumentVm
{
    public Guid Id { get; set; }
    public Guid LawId { get; set; }
    public string Path { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string DocumentName { get; set; } = default!;
}

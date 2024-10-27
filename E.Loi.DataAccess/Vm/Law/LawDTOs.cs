namespace E.Loi.DataAccess.Vm.Law;

public class LawDTOs
{
    public Guid LawId { get; set; }
    public string Label { get; set; } = default!;
    public int Year { get; set; }
    public string Number { get; set; } = default!;
    public string Category { get; set; } = default!;
}

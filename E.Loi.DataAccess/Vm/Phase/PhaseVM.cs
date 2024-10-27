namespace E.Loi.DataAccess.Vm.Phase;

public class PhaseVM
{
    public string Id { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string lawId { get; set; } = default!;
    public string status { get; set; } = default!;
    public int order { get; set; }
}

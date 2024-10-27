namespace E.Loi.DataAccess.Vm.Laws;

public class LawVm
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Number { get; set; } = string.Empty;
    public string CommissionName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string PhaseName { get; set; } = string.Empty;
    public string StatuName { get; set; } = string.Empty;
    public Guid? PhaseLawId { get; set; }
    public bool HasAction { get; set; }
    public List<string> PhaseIds { get; set; } = new List<string>();
    public DateTime CreationDate { get; set; }
    public DateTime SubmitedDate { get; set; }
}

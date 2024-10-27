namespace E.Loi.DataAccess.Vm.Statistics;

public class StatisticsVM
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public int TextLaws_Number { get; set; }
    public int LawProjects_Number { get; set; }
}

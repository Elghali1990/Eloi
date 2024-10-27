namespace E.Loi.DataAccess.Vm.Statistics;

public class StatisticsDtos
{
    public string StatuLabel { get; set; } = string.Empty;
    public int Counter { get; set; }
    public List<Guid> Ids { get; set; } = new List<Guid>();
}

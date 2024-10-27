namespace E.Loi.DataAccess.Vm.Node;
public class NodeVoteVm
{
    public Guid Id { get; set; }
    public Guid VoteId { get; set; } = Guid.Empty;
    public string? Label { get; set; }
    public int? InFavor { get; set; }
    public int? Against { get; set; }
    public int? Neutral { get; set; }
    public string? Result { get; set; }
    public string? ResultFr { get; set; }
    public DateTime? VoteDate { get; set; }
    public string? Observation { get; set; }
    public Guid? IdFinance { get; set; }
    public bool suppressed { get; set; } = false;
}

namespace E.Loi.DataAccess.Vm.Amendment;

public class SetAmendData
{
    public string lawNumber { get; set; } = default!;
    public int lawYear { get; set; }
    public Guid TeamId { get; set; }
    public List<Guid> amendmentsIds { get; set; }
    public string OutType { get; set; } = default!;
    public bool WhithVote { get; set; }
}

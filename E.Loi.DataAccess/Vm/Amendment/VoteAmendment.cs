namespace E.Loi.DataAccess.Vm.Amendment;

public class VoteAmendment
{
    public Guid Id { get; set; }
    public Guid? IdFinance { get; set; }
    public Guid NodeId { get; set; }
    public int? Infavor { get; set; }
    public int? Against { get; set; }
    public int? Neutral { get; set; }
    public string Result { get; set; } = default!;
    public string Observation { get; set; } = default!;
    public bool suppressed { get; set; }
}

//public class VoteDto
//{
//    public Guid id { get; set; }
//    public Guid entityId { get; set; }
//    public int? inFavor { get; set; }
//    public int? against { get; set; }
//    public int? neutral { get; set; }
//    public bool suppressed { get; set; }
//    public string observation { get; set; }
//    public string? result { get; set; }

//}
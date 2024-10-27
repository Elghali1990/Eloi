namespace E.Loi.DataAccess.Vm.Amendment;
public class AmendmentDetails
{
    public string? TeamName { get; set; }
    public int Number { get; set; }
    public string? AmendmentIntent { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Justification { get; set; }
    public string? OriginalContent { get; set; }
    public int? InFavor { get; set; }
    public int? Against { get; set; }
    public int? Neutral { get; set; }
    public string? Result { get; set; }
    public string? Observation { get; set; }
    public Guid NodeId { get; set; }

}


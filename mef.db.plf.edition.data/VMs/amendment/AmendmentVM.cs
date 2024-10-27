namespace E.Loi.Edition.Generation.VMs.amendment;

public class AmendmentVM
{
    public string Id { get; set; } = default!;
    public string NodeTitle { get; set; } = default!;
    public Guid NodeId { get; set; }
    public Guid NodeTypeId { get; set; }
    public string teamTitle { get; set; } = default!;
    public int number { get; set; }
    public int numberSystem { get; set; }
    public string amendmentIntent { get; set; } = default!;
    public string titleParagraphe { get; set; } = default!;
    public string content { get; set; } = default!;
    public string article { get; set; } = default!;
    public string title { get; set; } = default!;
    public string type { get; set; } = default!;
    public string justification { get; set; } = default!;
    public string originalContent { get; set; } = default!;
    public int ordre { get; set; }
    public int? inFavor { get; set; }
    public int? against { get; set; }
    public int? neutral { get; set; }
    public string? result { get; set; }
    public string? governmentPosition { get; set; }

    //public AmendmentListVM[] amendmentsReferences { get; set; }
    //public GovernmentPositionVM? governmentPositionVM { get; set; }
    //public VoteResultVM? voteResultVM { get; set; }
    //public Node node { get; set; }
}

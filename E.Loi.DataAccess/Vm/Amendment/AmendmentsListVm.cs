namespace E.Loi.DataAccess.Vm.Amendment;

public class AmendmentsListVm
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public int Order { get; set; }
    public string? NodeTitle { get; set; }
    public string? ParentNodeTitle { get; set; }
    public string? TitleParagraphe { get; set; }
    public Guid NodeId { get; set; }
    public Guid NodeTypeId { get; set; }
    public string? Title { get; set; }
    public string? AmendmentType { get; set; }
    public string? AmendmentIntent { get; set; }
    public string? AmendmentsStatus { get; set; }
    public string? Team { get; set; }
    public string? GovernmentPosition { get; set; }
    public string? VoteResult { get; set; }
    public DateTime? VotingDate { get; set; }
    public string? Content { get; set; }
    public string? NewContent { get; set; }
    public string? OriginalContent { get; set; }
    public string? Justification { get; set; }
    public int NumberSystem { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? SubmitedDate { get; set; }
    public bool IsOrderChanged { get; set; } = false;
    public bool IsAmendmentSession { get; set; }
    public bool IsAmendementRattrape { get; set; }

}

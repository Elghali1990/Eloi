namespace E.Loi.Entities.Models;

public class Amendment
{
    public Guid Id { get; set; }

    public Guid NodeId { get; set; }

    public Guid TeamId { get; set; }

    public int Number { get; set; }

    public bool Supressed { get; set; }

    public string AmendmentType { get; set; } = null!;

    public string AmendmentIntent { get; set; } = null!;

    public string AmendmentsStatus { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Article { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Justification { get; set; } = null!;

    public string ArticleRef { get; set; } = null!;

    public string? OriginalContent { get; set; }

    public Guid? ReferenceNodeId { get; set; }

    public Guid? CreatedFromId { get; set; }

    public int Ordre { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public virtual Guid ? CreatedFrom { get; set; }

    public virtual ICollection<GovernmentPosition> GovernmentPositions { get; set; } = new List<GovernmentPosition>();

    public virtual Node Node { get; set; } = null!;

    public virtual Node? ReferenceNode { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual ICollection<VoteAmendementResult> VoteAmendementResults { get; set; } = new List<VoteAmendementResult>();

    public virtual ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();

    public virtual ICollection<Amendment> RefAmendments { get; set; } = new List<Amendment>();
}

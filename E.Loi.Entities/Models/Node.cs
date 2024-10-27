namespace E.Loi.Entities.Models;

public class Node
{
    public Guid Id { get; set; }

    public string Label { get; set; } = null!;

    public Guid TypeId { get; set; }

    public string Content { get; set; } = null!;

    public string OriginalContent { get; set; } = null!;

    public Guid PresentationNote { get; set; }

    public int Counter { get; set; }

    public Guid PhaseLawId { get; set; }

    public Guid? ParentNodeId { get; set; }

    public Guid LawId { get; set; }

    public bool Checked { get; set; }

    public string Status { get; set; } = null!;

    public int Number { get; set; }

    public int Bis { get; set; }

    public int Order { get; set; }

    public Guid? CreatedFrom { get; set; }

    public Guid? BusinessRef { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public string? Nature { get; set; }

    public virtual ICollection<Amendment> AmendmentNodes { get; set; } = new List<Amendment>();

    public virtual ICollection<Amendment> AmendmentReferenceNodes { get; set; } = new List<Amendment>();

    public virtual ICollection<Node> InverseParentNode { get; set; } = new List<Node>();

    public virtual Law Law { get; set; } = null!;

    public virtual Node? ParentNode { get; set; }

    public virtual Phase PhaseLaw { get; set; } = null!;

    public virtual NodeType Type { get; set; } = null!;

    public virtual ICollection<VoteNodeResult> VoteNodeResults { get; set; } = new List<VoteNodeResult>();
}

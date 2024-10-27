namespace E.Loi.DataAccess.Models;
public partial class NodeType
{
    public Guid Id { get; set; }

    public string Label { get; set; } = null!;

    public bool HasContent { get; set; }

    public bool HasTitle { get; set; }

    public bool HasOriginalContent { get; set; }

    public bool HasPresentationNote { get; set; }

    public bool IsParentType { get; set; }

    public bool Reference { get; set; }

    public bool ShowLabel { get; set; }

    public bool ShowPrefix { get; set; }

    public bool Referencable { get; set; }

    public string ContentType { get; set; } = null!;

    public string OrderType { get; set; } = null!;

    public Guid FamillyId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public bool IsAmendableAdd { get; set; }

    public bool IsAmendableDelete { get; set; }

    public bool IsAmendableEdit { get; set; }

    public bool IsVotable { get; set; }
    public string? TextType { get; set; }
    public Guid? IdFinance { get; set; }
    public virtual NodeHierarchyFamilly Familly { get; set; } = null!;

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();

    public virtual ICollection<NodeType> Children { get; set; } = new List<NodeType>();

    public virtual ICollection<NodeType> Parents { get; set; } = new List<NodeType>();
}

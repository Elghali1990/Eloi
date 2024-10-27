namespace E.Loi.Entities.Models;

public class NodeHierarchyFamilly
{
    public Guid Id { get; set; }

    public string Label { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public virtual ICollection<Law> Laws { get; set; } = new List<Law>();

    public virtual ICollection<NodeType> NodeTypes { get; set; } = new List<NodeType>();
}

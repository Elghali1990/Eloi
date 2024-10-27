namespace E.Loi.Entities.Models;

public class Phase
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public int Order { get; set; }

    public Guid LawId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public virtual ICollection<Law> Laws { get; set; } = new List<Law>();

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();
}

namespace E.Loi.Entities.Models;

public class GovernmentPosition
{
    public Guid Id { get; set; }

    public Guid AmendmentId { get; set; }

    public string Position { get; set; } = null!;

    public string Justification { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public virtual Amendment Amendment { get; set; } = null!;
}

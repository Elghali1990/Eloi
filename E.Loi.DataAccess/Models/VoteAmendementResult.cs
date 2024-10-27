namespace E.Loi.DataAccess.Models;

public partial class VoteAmendementResult
{
    public Guid Id { get; set; }

    public Guid AmendmentId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public int? InFavor { get; set; }

    public int? Against { get; set; }

    public int? Neutral { get; set; }

    public bool Suppressed { get; set; }

    public string? Observation { get; set; }

    public string? Result { get; set; }

    public virtual Amendment Amendment { get; set; } = null!;
}
﻿namespace E.Loi.Entities.Models;

public class VoteNodeResult
{
    public Guid Id { get; set; }

    public Guid NodePhaseLawId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public int? InFavor { get; set; }

    public int? Against { get; set; }

    public int? Neutral { get; set; }

    public bool Suppressed { get; set; }

    public Guid Observation { get; set; }

    public string? Result { get; set; }

    public virtual Node NodePhaseLaw { get; set; } = null!;
}

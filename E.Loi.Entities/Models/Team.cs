namespace E.Loi.Entities.Models;

public class Team
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int Ordre { get; set; }

    public double Weight { get; set; }

    public bool IsMajority { get; set; }

    public string TeamType { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public string TeamEntity { get; set; } = null!;

    public virtual ICollection<Amendment> Amendments { get; set; } = new List<Amendment>();

    public virtual ICollection<Law> CommissionLaws { get; set; } = new List<Law>();

    public virtual ICollection<Law> TeamLaws { get; set; } = new List<Law>();

    public virtual ICollection<Team> MemberTeams { get; set; } = new List<Team>();

    public virtual ICollection<Team> ParentTeams { get; set; } = new List<Team>();
}

namespace E.Loi.DataAccess.Models;

public class LegislativeYear
{
    public Guid Id { get; set; }

    public string? Label { get; set; }

    public Guid? LegislativeId { get; set; }
    public virtual ICollection<Law> Laws { get; set; } = new List<Law>();

    public virtual Legislative? Legislative { get; set; }

    public virtual ICollection<LegislativeSession> LegislativeSessions { get; set; } = new List<LegislativeSession>();
}

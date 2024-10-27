namespace E.Loi.DataAccess.Models;

public class LegislativeSession
{
    public Guid Id { get; set; }

    public string? Label { get; set; }

    public Guid? IdYear { get; set; }

    public virtual LegislativeYear? IdYearNavigation { get; set; }
    public virtual ICollection<Law> Laws { get; set; } = new List<Law>();
}

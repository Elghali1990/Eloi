namespace E.Loi.DataAccess.Models;

public class Legislative
{
    public Guid Id { get; set; }

    public string? Label { get; set; }
    public virtual ICollection<Law> Laws { get; set; } = new List<Law>();
    public virtual ICollection<LegislativeYear> LegislativeYears { get; set; } = new List<LegislativeYear>();
}

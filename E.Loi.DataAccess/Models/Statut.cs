namespace E.Loi.DataAccess.Models;

public class Statut
{
    public Guid Id { get; set; }

    public string? Label { get; set; }

    public int? Order { get; set; }

    public Guid? PhaseId { get; set; }
    public virtual ICollection<Law> Laws { get; set; } = new List<Law>();
}

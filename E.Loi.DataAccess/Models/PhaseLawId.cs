namespace E.Loi.DataAccess.Models;

public class PhaseLawId
{
    public Guid Id { get; set; }

    public Guid? LawId { get; set; }

    public Guid? PhaseId { get; set; }

    public string? Statu { get; set; }

    public virtual Law? Law { get; set; }

    public virtual Phase? Phases { get; set; }
}

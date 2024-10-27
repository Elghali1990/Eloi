namespace E.Loi.Entities.Models;

public class Law
{
    public Guid Id { get; set; }

    public int Year { get; set; }

    public string Label { get; set; } = null!;

    public string Number { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Type { get; set; } = null!;

    public Guid NodeHierarchyFamillyId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public Guid? IdCommission { get; set; }

    public Guid? IdEquipe { get; set; }

    public DateTime DateAffectationBureau { get; set; }

    public DateTime DateAffectationCdc { get; set; }

    public DateTime DateAffectationCommission1 { get; set; }

    public DateTime DateAffectationCommission2 { get; set; }

    public DateTime DateAffectationSeance1 { get; set; }

    public DateTime DateAffectationSeance2 { get; set; }

    public DateTime DateFinAmendments1 { get; set; }

    public DateTime DateFinAmendments2 { get; set; }

    public DateTime DatePublication { get; set; }

    public DateTime DateRecu { get; set; }

    public DateTime DateTelechargement { get; set; }

    public string? Observations { get; set; }

    public Guid? PhaseLawId { get; set; }

    public virtual Team? IdCommissionNavigation { get; set; }

    public virtual Team? IdEquipeNavigation { get; set; }

    public virtual NodeHierarchyFamilly NodeHierarchyFamilly { get; set; } = null!;

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();

    public virtual Phase? PhaseLaw { get; set; }

    public virtual ICollection<Law> Laws { get; set; } = new List<Law>();

    public virtual ICollection<Law> RefLaws { get; set; } = new List<Law>();
}

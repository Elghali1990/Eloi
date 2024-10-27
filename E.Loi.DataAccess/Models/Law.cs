namespace E.Loi.DataAccess.Models;

public partial class Law
{
    public Guid Id { get; set; }

    public int Year { get; set; }

    public string Label { get; set; } = null!;

    public string Number { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Type { get; set; } = null!;

    public Guid? NodeHierarchyFamillyId { get; set; }

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

    public DateTime? DateFinAmendments1 { get; set; }

    public DateTime? DateFinAmendments2 { get; set; }

    public DateTime DatePublication { get; set; }

    public DateTime DateRecu { get; set; }

    public DateTime DateTelechargement { get; set; }

    public string? Observations { get; set; }

    public Guid? PhaseLawId { get; set; }

    public Guid? IdLegislative { get; set; }

    public Guid? LegislativeYearId { get; set; }

    public Guid? IdSession { get; set; }

    public string? TextOwner { get; set; }

    public DateTime? ProgrammedDateCommRead1 { get; set; }

    public DateTime? DateVoteCommRead1 { get; set; }

    public DateTime? DateVoteSeanceRead1 { get; set; }

    public DateTime? DateAffectationBureau2 { get; set; }

    public DateTime? ProgrammedDateCommRead2 { get; set; }

    public DateTime? DateVoteCommRead2 { get; set; }

    public DateTime? DateVoteSeanceRead2 { get; set; }

    public int? PublishNumber { get; set; }
    public Guid? StatuId { get; set; }
    public bool? DisplayForCommission { get; set; }

    public bool? DisplayForSession { get; set; }
    public Guid? IdFinance { get; set; }
    public virtual Team? Commission { get; set; }

    public virtual Team? Team { get; set; }

    public virtual Legislative? IdLegislativeNavigation { get; set; }

    public virtual LegislativeSession? IdSessionNavigation { get; set; }

    public virtual NodeHierarchyFamilly NodeHierarchyFamilly { get; set; } = null!;

    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();
    public virtual ICollection<PhaseLawId> PhaseLawIds { get; set; } = new List<PhaseLawId>();
    public virtual Phase? PhaseLaw { get; set; }
    public virtual Statut? Statu { get; set; }

    public virtual ICollection<Law> Laws { get; set; } = new List<Law>();

    public virtual ICollection<Law> RefLaws { get; set; } = new List<Law>();
}

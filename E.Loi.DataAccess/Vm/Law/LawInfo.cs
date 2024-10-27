namespace E.Loi.DataAccess.Vm.Law;

public class LawInfo
{
    public Guid LawId { get; set; }
    public Guid LegId { get; set; }
    public Guid YearId { get; set; }
    public Guid SessionId { get; set; }
    public string? LawLabel { get; set; }
    public Guid IdTeam { get; set; }
    public Guid IdStatu { get; set; }
    public DateTime? DateAffectationBureau { get; set; }
    public Guid CommissionId { get; set; }
    public DateTime? DateAffectationCommission1 { get; set; }
    public DateTime? ProgrammedDateCommRead1 { get; set; }
    public DateTime? DateFinAmendments1 { get; set; }
    public DateTime? DateVoteCommRead1 { get; set; }
    public DateTime? DateAffectationSeance1 { get; set; }
    public DateTime? DateVoteSeanceRead1 { get; set; }
    public DateTime? DateAffectationCdc { get; set; }
    public DateTime? DateAffectationBureau2 { get; set; }
    public DateTime? DateAffectationCommission2 { get; set; }
    public DateTime? ProgrammedDateCommRead2 { get; set; }
    public DateTime? DateFinAmendments2 { get; set; }
    public DateTime? DateVoteCommRead2 { get; set; }
    public DateTime? DateAffectationSeance2 { get; set; }
    public DateTime? DateVoteSeanceRead2 { get; set; }
    public DateTime? PublishedDate { get; set; }
    public int? Number { get; set; }
    public Guid UserId { get; set; }
}

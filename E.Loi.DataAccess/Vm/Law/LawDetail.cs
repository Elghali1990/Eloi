namespace E.Loi.DataAccess.Vm.Law;

public class LawDetail
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Number { get; set; } = string.Empty;
    public string PhaseName { get; set; } = string.Empty;
    public string TeameName { get; set; } = string.Empty;
    public string CommissionName { get; set; } = string.Empty;
    public DateTime? DateAffectaionBureau_1 { get; set; }
    public DateTime? DateAffectaionCommission_1 { get; set; }
    public DateTime? DateProgramationCommission_1 { get; set; }
    public DateTime? DateVoteCommission_1 { get; set; }
    public DateTime? DateVoteSession_1 { get; set; }
    public DateTime? DateAffectaionBureau_2 { get; set; }
    public DateTime? DateAffectaionCommission_2 { get; set; }
    public DateTime? DateProgramationCommission_2 { get; set; }
    public DateTime? DateVoteCommission_2 { get; set; }
    public DateTime? DateVoteSession_2 { get; set; }
}

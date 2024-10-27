namespace E.Loi.DataAccess.Vm.Amendment;

public class SetAmendmentStatuVm
{
    public List<Guid> Ids { get; set; }
    public Guid UserId { get; set; }
    public string AmendmentStatu { get; set; } = default!;
}

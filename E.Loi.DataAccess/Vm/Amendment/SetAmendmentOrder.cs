namespace E.Loi.DataAccess.Vm.Amendment;

public class SetAmendmentOrder
{
    public List<AmendmentOrder> AmendmentOrders { get; set; } = default(List<AmendmentOrder>)!;
    public Guid LastModifiedBy { get; set; }
}

public class AmendmentOrder
{
    public Guid Id { get; set; }
    public int Order { get; set; }
}

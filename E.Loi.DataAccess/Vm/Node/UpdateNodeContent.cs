namespace E.Loi.DataAccess.Vm.Node;

public class UpdateNodeContent
{
    public Guid NodeId
    {
        get;
        set;
    }

    public string NewContent
    {
        get;
        set;
    } = string.Empty;

    public string OldContent
    {
        get;
        set;
    } = string.Empty;

    public Guid LastModifiedBy
    {
        get;
        set;
    }
}

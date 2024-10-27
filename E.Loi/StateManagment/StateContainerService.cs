namespace E.Loi.StateManagment;

public class StateContainerService
{
    public StateContainerVm? state { get; set; } = new();
    public UserVm user { get; set; } = new();
    public AmendmentTypes amendmentType { get; set; }
}

namespace E.Loi.StateManagment;

public class StateContainerVm
{
    public NodeVm NodeVm
    {
        get;
        set;
    } = new NodeVm();

    public bool ShowViewNodeContent
    {
        get;
        set;
    } = false;

    public bool ShowAddAmendment
    {
        get;
        set;
    } = false;

    public bool ShowViewListAmendments
    {
        get;
        set;
    } = false;

    public bool ShowViewAllAmendments
    {
        get;
        set;
    } = false;

    public bool ShowSubmitedAmendments
    {
        get;
        set;
    } = false;

    public bool HasRightAddAmendment
    {
        get;
        set;
    } = false;

    public bool HasRightvoteAmendment
    {
        get;
        set;
    } = false;

    public bool HasRightAddConsensusHarmonization
    {
        get;
        set;
    } = false;

    public bool HasRightShowViewListAmendments
    {
        get;
        set;
    } = false;

    public bool HasRightUploadAmendments
    {
        get;
        set;
    } = false;

    public bool ShowTeamAmendments
    {
        get;
        set;
    } = true;


    public bool HasRightAddAmendmentSupplementary
    {
        get;
        set;
    } = true;

    public bool ShowAmendmentsSupplementary
    {
        get;
        set;
    }

}

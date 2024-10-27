namespace E.Loi.DataAccess.Vm.Amendment;

public class AmendmentVm
{
    public Guid NodeId
    {
        get;
        set;
    }

    public Guid TeamId
    {
        get;
        set;
    }

    public string TeamName
    {
        get;
        set;
    } = string.Empty;

    public int Number
    {
        get;
        set;
    }

    public string AmendmentIntent
    {
        get;
        set;
    } = string.Empty;

    public string AmendmentType
    {
        get;
        set;
    } = string.Empty;

    public string Title
    {
        get;
        set;
    } = string.Empty;

    public string Content
    {
        get;
        set;
    } = string.Empty;

    public string Justification
    {
        get;
        set;
    } = string.Empty;

    public string OriginalContent
    {
        get;
        set;
    } = string.Empty;

    public Guid CreatedBy
    {
        get;
        set;
    }

    public Guid UpdatedBy
    {
        get;
        set;
    }

    public List<Guid>? AmendmentIds
    {
        get;
        set;
    }

    public bool IsAmendementSeance
    {
        get;
        set;
    } = false;

    public bool IsAmendementRattrape
    {
        get;
        set;
    } = false;

    public string ParagreapheTitle
    {
        get;
        set;
    } = string.Empty;

    public int ParagrapheOrder
    {
        get;
        set;
    }

}

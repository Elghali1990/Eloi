namespace E.Loi.DataAccess.Dtos;

public class AmendmentSession
{
    //public string? ParentNodeLabel
    //{
    //    get;
    //    set;
    //}

    //public string ? NodeLabel 
    //{ 
    //    get; 
    //    set; 
    //}

    public Guid TeamId
    {
        get;
        set;
    }

    public string? TeamName
    {
        get;
        set;
    }

    public string? AmendmentIntent
    {
        get;
        set;
    }

    public VoteSessionDto vote
    {
        get;
        set;
    }
    //public string ? OriginalContent 
    //{ 
    //    get; 
    //    set; 
    //}

    //public string ? Content 
    //{ 
    //    get; 
    //    set; 
    //}

    //public string ? Justification 
    //{ 
    //    get; 
    //    set; 
    //}
    //public string? AmendmentIntent
    //{
    //    get;
    //    set;
    //}

    public int Order
    {
        get;
        set;
    }

    public int Number
    {
        get;
        set;
    }

    public int NumberSystem
    {
        get;
        set;
    }
}

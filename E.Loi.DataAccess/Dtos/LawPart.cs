namespace E.Loi.DataAccess.Dtos;

public class LawPart
{
    public string ? Label
    {
        get;
        set;
    }

    public List<NodePart> NodesPart
    {
        get;
        set;
    } = new();


}


public class NodePart
{
    public Guid Id
    {
        get;
        set;
    }
    public string ? Parent
    {
        get;
        set;
    }
    public string ? SubPrent
    {
        get;
        set;
    }

    public string ? Label 
    {
        get;
        set;
    }

    public List<AmendmentTeam> Teams
    {
        get;
        set;
    }

    public int AmendmentsCount
    {
        get;
        set;
    }
}

public class AmendmentTeam
{
    public Guid Id
    {
        get;
        set;
    }

    public string ? TeamName
    {
        get;
        set;
    }

    public int AmendmentCount
    {
        get;

        set;
    }
}





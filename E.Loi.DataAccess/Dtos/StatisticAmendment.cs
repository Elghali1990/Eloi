namespace E.Loi.DataAccess.Dtos;

public class StatisticAmendment
{
    public string? TeamName
    {
        get;
        set;
    }

    public int Rejected
    {
        get;
        set;
    }

    public int Accepted
    {
        get;
        set;
    }

    public int Suppressed
    {
        get;
        set;
    }
    public int Totale
    {
        get;
        set;
    }
}


public class StatisticDto
{
    public string SatatisticType
    {
        get;
        set;
    }

    public List<StatisticAmendment> StatisticAmendments
    {
        get;
        set;
    } = new List<StatisticAmendment>();
}
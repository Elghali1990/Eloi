namespace E.Loi.DataAccess.Dtos;

public class VoteResultDto
{
    public string? NodeLabel
    {
        get;
        set;
    }

    public string? TeamName
    {
        get;
        set;
    }

    public Guid TeamId
    {
        get;
        set;
    }
    public ParentNode? ParentNode
    {
        get;
        set;
    }

    public int Number
    {
        get;
        set;
    }

    public int InFavor
    {
        get;
        set;
    }
    public int Against
    {
        get;
        set;
    }

    public int Neutral
    {
        get;
        set;
    }

    public string? Result
    {
        get;
        set;
    }

    public string? Observation
    {
        get;
        set;
    }
}

public class ParentNode
{
    public string? Parent
    {
        get;
        set;
    }
    public string? Child
    {
        get;
        set;
    }
}

public class VoteAmendmentsTeam
{
    public List<TeamLaw> TeamLaws
    {
        get;
        set;
    } = new List<TeamLaw>();

    public List<VoteResultDto> VoteResult
    {
        get;
        set;
    } = new List<VoteResultDto>();
}

public class TeamLaw
{
    public string? TeamName
    {
        get;
        set;
    }

    public string? LawName
    {
        get;
        set;
    }
}
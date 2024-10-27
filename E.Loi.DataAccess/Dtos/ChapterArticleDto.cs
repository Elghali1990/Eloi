namespace E.Loi.DataAccess.Dtos;

public class ChapterArticleDto : ChapterDto
{
    public List<ChapterItem> chapters { get; set; }
}

public class ChapterItem
{
    public Guid Id
    {
        get;
        set;
    }
    public Guid CreatedFrom
    {
        get;
        set;
    }
    public string? Label
    {
        get;
        set;
    }

    public List<Children> Childrens
    {
        get;
        set;
    }
    public VoteSessionDto? vote
    {
        get;
        set;
    }

    public bool NodeIsChanged
    {
        get;
        set;
    }

    //public List<AmendmentSession> Amendments
    //{
    //    get;
    //    set;
    //} = new List<AmendmentSession>();
}


public class Children
{
    public Guid Id
    {
        get;
        set;
    }
    public string Label
    {
        get;
        set;
    }
    public List<AmendmentSession> Amendments
    {
        get;
        set;
    }
}
namespace E.Loi.DataAccess.Dtos;

public class SectionDto
{
    public string? SectionNumber
    {
        get;
        set;
    }

    public string? SectionName
    {
        get;
        set;
    }

    public List<ChapterDto> RubricList
    {
        get;
        set;
    } = new List<ChapterDto>();

    public List<ChapterArticleDto> ArticleList
    {
        get;
        set;
    } = new List<ChapterArticleDto>();
}

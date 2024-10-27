

namespace E.Loi.DataAccess.Dtos;

public class VoteSessionDto
{
    public int? InFavor { get; set; }
    public int? Against { get; set; }
    public int? Neutral { get; set; }
    public string? Result { get; set; }
}

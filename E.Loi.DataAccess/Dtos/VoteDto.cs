namespace E.Loi.DataAccess.Dtos;

public class VoteDto
{
    public Guid id { get; set; }
    public Guid entityId { get; set; }
    public int? inFavor { get; set; }
    public int? against { get; set; }
    public int? neutral { get; set; }
    public bool suppressed { get; set; }
    public string? observation { get; set; }
    public string? result { get; set; }
}


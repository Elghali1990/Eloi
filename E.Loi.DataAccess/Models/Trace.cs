namespace E.Loi.DataAccess.Models;

public class Trace
{
    public Guid Id { get; set; }

    public string? Operation { get; set; }

    public DateTime? DateOperation { get; set; }

    public Guid? UserId { get; set; }
}

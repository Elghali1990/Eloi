namespace E.Loi.DataAccess.Models;

public class AmendmentContent
{
    public Guid Id { get; set; }

    public Guid? AmendmentId { get; set; }

    public string? Content { get; set; }

    public virtual Amendment? Amendment { get; set; }
}

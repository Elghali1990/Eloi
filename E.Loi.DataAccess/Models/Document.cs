namespace E.Loi.DataAccess.Models;
public partial class Document
{
    public Guid Id { get; set; }

    public string Path { get; set; } = null!;

    public Guid LawId { get; set; }
    public string? Type { get; set; }
    public virtual Law Law { get; set; } = null!;
    public string? DocumentName { get; set; }
}
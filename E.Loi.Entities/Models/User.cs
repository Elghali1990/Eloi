namespace E.Loi.Entities.Models;

public class User
{
    public Guid Id { get; set; }

    public string ExternalId { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Occupation { get; set; } = null!;

    public string Structure { get; set; } = null!;

    public string Hash { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public string Salt { get; set; } = null!;
}

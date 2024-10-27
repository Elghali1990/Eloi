namespace E.Loi.DataAccess.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string ExternalId { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Occupation { get; set; } = null!;

    public string Structure { get; set; } = null!;

    public string? Hash { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime ModifictationDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid LastModifiedBy { get; set; }

    public string? Salt { get; set; } = null!;

    public Guid? TeamId { get; set; }

    public virtual Team? Team { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}

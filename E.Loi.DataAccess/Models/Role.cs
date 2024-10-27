namespace E.Loi.DataAccess.Models;

public partial class Role
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Label { get; set; }
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

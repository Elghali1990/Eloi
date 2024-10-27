using E.Loi.Helpers.Enumarations;

namespace E.Loi.DataAccess.Vm.Team;

public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Ordre { get; set; }
    public int Weight { get; set; }
    public bool IsMajority { get; set; }
    public Guid PlfId { get; set; }
    public TeamTypes TeamType { get; set; }
    public TeamEntities TeamEntitie { get; set; }
}



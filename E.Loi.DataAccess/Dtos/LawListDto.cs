namespace E.Loi.DataAccess.Dtos;

public class LawListDto
{
    public Guid Id { get; set; }
    public string? Label { get; set; }
    public List<PhaseDto>? Phases { get; set; }

}

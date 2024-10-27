using E.Loi.Helpers.Enumarations;

namespace E.Loi.DataAccess.Dtos;

public class AmendmentDto
{
    public Guid Id { get; set; }
    public Guid NodeId { get; set; }
    public Guid TeamId { get; set; }
    public int Number { get; set; }
    public AmendmentTypes AmendmentType { get; set; }
    public AmendmentIntents AmendmentIntent { get; set; }
    public AmendmentsStatus AmendmentsStatus { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Justification { get; set; } = null!;
    public string OriginalContent { get; set; } = null!;
    public int Ordre { get; set; }
    public NodeDto? Node { get; set; }
    public string[]? amendmentsRefIds { get; set; }
}

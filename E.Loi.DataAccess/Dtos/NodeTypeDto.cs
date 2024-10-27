namespace E.Loi.DataAccess.Dtos;
public class NodeTypeDto
{
    public Guid Id { get; set; }
    public string Label { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public bool IsAmendableAdd { get; set; }
    public bool IsAmendableDelete { get; set; }
    public bool IsAmendableEdit { get; set; }
    public bool IsVotable { get; set; }

}

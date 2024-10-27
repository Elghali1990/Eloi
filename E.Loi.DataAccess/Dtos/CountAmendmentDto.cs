namespace E.Loi.DataAccess.Dtos;

public class CountAmendmentDto
{
    public string? TeamName { get; set; }
    public int Count { get; set; }
    public DateTime? SubmitedDate { get; set; }
    public string? SubmitedBy { get; set; }
    public DateTime? PuplishedDate { get; set; }
    public string? PuplishedBy { get; set; }
    public DateTime? VotingDate { get; set; }
    public string? VotingBy { get; set; }

}

namespace E.Loi.Edition.Generation.Generation.amendment.ViewModels;

public class AmendmentsTemplate
{
    public string Team { get; set; } = default!;
    public string LabelNode { get; set; } = default!;
    public string AmendmentTitle { get; set; } = default!;
    public int Number { get; set; }
    public int NumberSystem { get; set; }
    public string Intent { get; set; } = default!;
    public string OriginalContent { get; set; } = default!;
    public string Content { get; set; } = default!;
    public string Justification { get; set; } = default!;
}

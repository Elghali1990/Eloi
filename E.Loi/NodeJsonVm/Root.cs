namespace E.Loi.NodeJsonVm;

public class Root
{
    public string version { get; set; } = default!;
    public List<Row> rows { get; set; } = new();
    public string label { get; set; } = default!;
    public int lastModification { get; set; }
}

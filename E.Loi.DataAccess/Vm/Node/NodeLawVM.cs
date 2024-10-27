namespace E.Loi.DataAccess.Vm.Node;

public class NodeLawVM
{
    public string id { get; set; }
    public string? label { get; set; }
    public string? labelPrint { get; set; }
    public string? type { get; set; }
    public string? typeLabel { get; set; }
    public string? status { get; set; }
    public string? nature { get; set; }
    public int number { get; set; }
    public int bis { get; set; }
    public int order { get; set; }
    public string? parentNode { get; set; }
    public NodeLawVM? parent { get; set; }
    public NodeLawVM[] children { get; set; }
}

using E.Loi.DataAccess.Vm.Law;
using E.Loi.DataAccess.Vm.Node;
using E.Loi.DataAccess.Vm.Phase;
using E.Loi.Edition.Generation.VMs.NodeType;

namespace E.Loi.Edition.Generation.VMs;
public class SetDocumentData
{
    public string name { get; set; } = default!;
    //  public string version { get; set; }
    public string format { get; set; } = default!;
    public NodeLawVM[] nodeSelected { get; set; }
    public NodeLawVM[] nodes { get; set; }
    public NodeTypeVM[] nodeTypes { get; set; }
    public string language { get; set; } = default!;
    public PhaseVM phase { get; set; }
    public LawVM law { get; set; }
    public bool? printings { get; set; }
    public HashSet<Guid> authorised_nodes { get; set; }
    public HashSet<Guid> authorised_amendements { get; set; }
    // public TeamVM?[] teams { get; set; }
    public string? typePartie { get; set; }
    public string? typeArticle { get; set; }

}


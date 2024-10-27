namespace E.Loi.Edition.Generation.VMs.NodeType;

public class NodeTypeVM
{
    public string id { get; set; } = default!;
    public string label { get; set; } = default!;

    public string contentType { get; set; } = default!;
    public bool hasContent { get; set; }
    public bool hasTitle { get; set; }
    public bool hasOriginalContent { get; set; }
    public bool hasPresentationNote { get; set; }
    public bool isParentType { get; set; }
    public bool isReferencable { get; set; }
    public string orderType { get; set; } = default!;

    public bool isAmendableEdit { get; set; }
    public bool isAmendableAdd { get; set; }
    public bool isAmendableDelete { get; set; }
    public bool isVotable { get; set; }

    public List<string> children { get; set; }
}

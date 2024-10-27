namespace E.Loi.Components.Layout;

public partial class EditLawTreeView
{
    /*
     * === Global Variables For Component ===  
    */
    [Parameter] public List<NodeVm>? nodes { get; set; }
    [Parameter] public int Level { get; set; } = 1;
    [Parameter] public EventCallback<NodeVm> OnSelect { get; set; }
    [Parameter] public EventCallback<Guid> OnAddNewNode { get; set; }
    [Parameter] public EventCallback<NodeVm> OnDeleteNode { get; set; }
    [Parameter] public EventCallback<NodeVm> OnEditNode { get; set; }
    List<NodeTypeVm> nodeTypes = new();


    /*
     * === Event Handle Selected Node ===
    */
    private async Task ItemSelected(NodeVm selectedItem)
    {
        if (Level == 1)
            UpdateSelection(nodes!, selectedItem);
        await OnSelect.InvokeAsync(selectedItem);
    }

    private void UpdateSelection(List<NodeVm> items, NodeVm selectedItem)
    {
        foreach (var item in items)
        {
            item.IsSelected = item == selectedItem;
            if (item.HasChildren)
                UpdateSelection(item.childrens!, selectedItem);
        }
    }

    private async Task AddNewNode(Guid Id) => await OnAddNewNode.InvokeAsync(Id);
    private async Task DeleteNode(NodeVm node) => await OnDeleteNode.InvokeAsync(node);
    private async Task EditNode(NodeVm node) => await OnEditNode.InvokeAsync(node);
}

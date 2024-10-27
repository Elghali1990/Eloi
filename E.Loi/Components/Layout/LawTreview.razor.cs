namespace E.Loi.Components.Layout;

public partial class LawTreview
{

    [Parameter]
    public List<NodeVm>? nodes
    {
        get;
        set;
    }

    [Parameter]
    public int Level
    {
        get;
        set;
    } = 1;

    [Parameter]
    public EventCallback<NodeVm> OnSelect
    {
        get;
        set;
    }
    private async Task ItemSelected(NodeVm selectedItem)
    {
        if (Level == 1)
        {
            UpdateSelection(nodes!, selectedItem);
        }
        await OnSelect.InvokeAsync(selectedItem);
    }

    private void UpdateSelection(List<NodeVm> items, NodeVm selectedItem)
    {
        foreach (var item in items)
        {
            item.IsSelected = item == selectedItem;
            if (item.HasChildren)
            {
                UpdateSelection(item.childrens!, selectedItem);
            }
        }
    }
}

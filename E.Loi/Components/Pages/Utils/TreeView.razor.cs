namespace E.Loi.Components.Pages.Utils;

public partial class TreeView
{

    [Parameter]
    public List<NodeVm>? Items
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

    [Parameter]
    public EventCallback<NodeVm> handleEventViewNodeContent
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<NodeVm> handleEventDisplayAmendmentList
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<NodeVm> handleEventShowSubmitedAmendments
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<NodeVm> handleEventDisplayAllAmendment
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<NodeVm> handleEventAddAmendment
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<NodeVm> handleEventAddAmendmentSupplementary
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<NodeVm> handleEventShowAmendmentsSupplementary
    {
        get;
        set;
    }


    [Parameter]
    public EventCallback<NodeVm> handleEventAddAmendmentConsensus
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<NodeVm> handleEventAddAmendmentHarmonization
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<NodeVm> handlePrintNodeContent
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<bool> handleVoteNode
    {
        get;
        set;
    }

    [Parameter]
    public EventCallback<bool> handleUploadAmendments
    {
        get;
        set;
    }

    public async Task EventViewNodeContent(NodeVm selectedNode)
    {
        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowSubmitedAmendments = false,
            ShowAddAmendment = false,
            ShowViewNodeContent = true,
            ShowViewAllAmendments = false
        };
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventViewNodeContent.InvokeAsync(selectedNode);
    }

    public async Task EventShowSubmitedAmendments(NodeVm selectedNode)
    {
        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowSubmitedAmendments = true,
            ShowAddAmendment = false,
            ShowViewNodeContent = false,
            ShowViewAllAmendments = false
        };
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventShowSubmitedAmendments.InvokeAsync(selectedNode);
    }
    public async Task EventhandleVoteNode()
    {
        await handleVoteNode.InvokeAsync(true);
    }
    public async Task EventhandleUploadAmendments()
    {
        await handleUploadAmendments.InvokeAsync(true);
    }

    public async Task EventPrintNodeContent(NodeVm selectedNode)
    {
        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowSubmitedAmendments = false,
            ShowAddAmendment = false,
            ShowViewNodeContent = false,
            ShowViewAllAmendments = false
        };
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handlePrintNodeContent.InvokeAsync(selectedNode);
    }
    public async Task EventDisplayAmendmentList(NodeVm selectedNode)
    {

        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = true,
            ShowAddAmendment = false,
            ShowViewNodeContent = false,
            ShowViewAllAmendments = false,
            ShowSubmitedAmendments = false
        };
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventDisplayAmendmentList.InvokeAsync(selectedNode);
    }
    public async Task EventAddAmendment(NodeVm selectedNode)
    {
        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowAddAmendment = true,
            ShowViewNodeContent = false,
            ShowViewAllAmendments = false,
            ShowSubmitedAmendments = false
        };
        stateContainerService.amendmentType = AmendmentTypes.SINGLE;
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventAddAmendment.InvokeAsync(selectedNode);

    }
    public async Task EventAddAmendmentSupplementary(NodeVm selectedNode)
    {
        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowAddAmendment = true,
            ShowViewNodeContent = false,
            ShowViewAllAmendments = false,
            ShowSubmitedAmendments = false
        };
        stateContainerService.amendmentType = AmendmentTypes.SINGLE;
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventAddAmendmentSupplementary.InvokeAsync(selectedNode);

    }

    public async Task EventShowAmendmentsSupplementary(NodeVm selectedNode)
    {
        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowAddAmendment = false,
            ShowViewNodeContent = false,
            ShowViewAllAmendments = false,
            ShowSubmitedAmendments = false,
            ShowAmendmentsSupplementary = true
        };
        stateContainerService.amendmentType = AmendmentTypes.SINGLE;
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventShowAmendmentsSupplementary.InvokeAsync(selectedNode);

    }
    public async Task EventDisplayAllAmendment(NodeVm selectedNode)
    {

        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowViewAllAmendments = true,
            ShowAddAmendment = false,
            ShowViewNodeContent = false,
            ShowSubmitedAmendments = false
        };
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventDisplayAllAmendment.InvokeAsync(selectedNode);
    }
    public async Task EventAddAmendmentConsensus(NodeVm selectedNode)
    {
        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowAddAmendment = true,
            ShowViewNodeContent = false,
            ShowViewAllAmendments = false,
            ShowSubmitedAmendments = false,
        };
        stateContainerService.amendmentType = AmendmentTypes.CONSENSUS;
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventAddAmendmentConsensus.InvokeAsync(selectedNode);

    }
    public async Task EventAddAmendmentHarmonization(NodeVm selectedNode)
    {
        stateContainerService.state = new()
        {
            NodeVm = selectedNode,
            ShowViewListAmendments = false,
            ShowAddAmendment = true,
            ShowViewNodeContent = false,
            ShowViewAllAmendments = false,
            ShowSubmitedAmendments = false
        };
        stateContainerService.amendmentType = AmendmentTypes.HARMONIZATION;
        if (Level == 1)
        {
            UpdateSelection(Items!, selectedNode);
        }
        await handleEventAddAmendmentHarmonization.InvokeAsync(selectedNode);

    }

    private void UpdateSelection(List<NodeVm> items, NodeVm selectedNode)
    {
        foreach (var item in items)
        {
            item.IsSelected = item == selectedNode;
            if (item.HasChildren)
            {
                UpdateSelection(item.childrens!, selectedNode);
            }
        }
    }

}

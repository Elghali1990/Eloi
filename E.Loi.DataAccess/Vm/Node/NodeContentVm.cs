namespace E.Loi.DataAccess.Vm.Node;

public class NodeContentVm
{
    public Guid Id 
    { 
        get; 
        set; 
    }

    public Guid NodeTypeId 
    { 
        get; 
        set; 
    }

    public string Label 
    { 
        get; 
        set; 
    } = default!;

    public string NodeContent 
    { 
        get; 
        set; 
    } = default!;

    public Guid? IdFinace 
    { 
        get; 
        set; 
    }

    public int Number
    {
        get;
        set;
    }

    public int Bis
    {
        get;
        set;
    }

}

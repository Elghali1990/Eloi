using AntDesign;
namespace E.Loi.Components.Layout;

public partial class BaseComponent : ComponentBase
{
    #region Globals Variabls
    public ITable? table;
    public int _pageIndex = 1;
    public int _pageSize = 10;
    public int _pageSize_2 = 20;
    public int _total = 0;
    public List<string> RequiredFieldsErrors = new List<string>();
    #endregion
}

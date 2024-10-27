namespace E.Loi.DataAccess.Vm.Node
{
    public class NodeTitleVM
    {
        public string Id { get; set; } = default!;
        public string Label { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string status { get; set; } = default!;
        public int number { get; set; }
        public int bis { get; set; }
        public int order { get; set; }
        public string? parentNode { get; set; }

        public NodeTitleVM? parent { get; set; }
        public NodeTitleVM[] children { get; set; }
    }
}

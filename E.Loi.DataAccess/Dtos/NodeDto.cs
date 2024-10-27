using E.Loi.Helpers.Enumarations;

namespace E.Loi.DataAccess.Dtos;

public class NodeDto
{
    //    public Guid Id { get; set; }
    //    public string Label { get; set; }
    //    public string Content { get; set; }
    //    public string OriginalContent { get; set; }
    //    public string PresentationNote { get; set; }
    //    public Guid? ParentNodeId { get; set; }
    //    public Guid TypeId { get; set; }
    //    public NodeStatus Status { get; set; }
    //    public int Number { get; set; }
    //    public int Bis { get; set; }
    //    public int Order { get; set; }
    //    public NodeOrigin ? Nature { get; set; }
    //    public bool IsDeleted { get; set; }
    //    public NodeDto[] ? Childrens { get; set; }
    //    //public NodeDto? ParentNode { get; set; }
    //    public NodeHeader[]? ParentNodes { get; set; }
    public Guid Id { get; set; }
    public string Label { get; set; }
    public string Content { get; set; }
    public string OriginalContent { get; set; }
    public string PresentationNote { get; set; }
    public Guid? ParentNodeId { get; set; }
    public Guid TypeId { get; set; }
    public NodeStatus Status { get; set; }
    public int Number { get; set; }
    public int Bis { get; set; }
    public int Order { get; set; }
    public NodeOrigin? Nature { get; set; }
    public bool IsDeleted { get; set; }
    public NodeDto[]? Childrens { get; set; }
    public NodeDto? ParentNode { get; set; }
}
//}

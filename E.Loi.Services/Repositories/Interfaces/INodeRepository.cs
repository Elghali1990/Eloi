namespace E.Loi.Services.Repositories.Interfaces;

public interface INodeRepository : IBaseRepository<Node>
{
    Task<Node> CreateVirtuelNode(CreateNodeVm model);
    Task<Node> CreateNode(CreateNodeVm model);
    Task<Node> GetNodeByID(Guid Id);
    Task<string> GetNodeLabel(Node node);
    Task<ServerResponse> UpdateNodeContent(NodeContentVm nodeContent, Guid UserId);
    Task<ServerResponse> DeleteNode(Guid nodeId, Guid userId);
    Task<NodeVm[]> GetNodes(Guid IdLaw, Guid PhaseId, bool includeVirtuelNode);
    Task<List<NodeCounter>> GetDirecteChilds(Guid ParentId);
    Task<NodeContentVm> GetNodeContent(Guid NodeId);
    Task<NodeVoteVm[]> GetFlatNodes(Node pnode, Guid? IdLaw, Guid? PhaseLawId);
    Task<ServerResponse> CloneNodes(Guid LawId, Guid CurentPhase, Guid DestinationPhase, string Statu);
    Task<FlatNode[]> GetFlatParents(Guid? nodeId, Node node);
    Task<FlatNode[]> GetLawSections(Guid Lawid, Guid PhaseLawId, bool IsPrinVotingFile);
    Task<List<FlatNode>> GetFlatPrint(Guid LawId, Guid PhaseLawId, Node node);
    Task<ServerResponse> CreateLawNodes(List<TextLaw> texts, Guid LawId, Guid CreatedBy);
    Task<ServerResponse> SetPhaseNodes(Guid LawId, Guid DestinationPhase, Guid LastModifiedBy, int Order);
    Task<Node_VM> GetNode(Node node);
    Task<NodeTitleVM> GetParent(Node node);
    Task<NodeTitleVM[]> GetDirectChildren(Node pnode);
    Task<NodeLawVM[]> GetNodesLawVM(Node node);
    Task<ServerResponse> CreateNodesLawAsync(DataAccess.Dtos.NodeDto[] nodesDto, Guid LawId);
    Task<NodeVm> GetParentNode(Guid lawId, Guid phaseLawId);
    Task<ServerResponse> cloneSectionWithChildrens(Guid sectionId, Guid destinationPhase);
    Task<NodeVoteVm[]> GetSectionWithNodes(Guid section, Node pnode);
    Task<SectionDto> GetSectionRubric(Guid sectionId);
    Task<List<NodeVoteVm>> GetSectionForVoting(Guid sectionId);
    Task<ServerResponse> SetNewContent(UpdateNodeContent Node);
    Task<List<LawPart>> GetNodeWithAmendmentForPresident(Guid LawId ,Guid PhaseId);
    Task<ServerResponse> GetNodeByTypeByNumberByBis(Guid ParentNodeId, Guid TypeId, int Number, int Bis);
}

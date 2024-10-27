using Microsoft.AspNetCore.Mvc;

namespace E.Loi.Repositories;

public interface INodeRepository
{
    Task<Node> CreateVirtuelNode(CreateNodeVm model);
    Task<Node> CreateNode(CreateNodeVm model);
    Task<Node> GetNodeByID(Guid Id);
    Task<ServerResponse> DeleteNode(Guid nodeId, Guid userId);
    Task<List<NodeCounter>> GetDirecteChilds(Guid ParentId);
    Task<List<NodeVm>> GetRecursiveChildren(Guid IdLaw, Guid PhaseId, bool includeVirtuelNode);
    Task<NodeContentVm> GetNodeContent(Guid NodeId);
    Task<NodeVoteVm[]> GetFlatNodes(Guid? IdLaw, Guid? PhaseLawId);
    Task<FlatNode[]> GetFlatParents(Guid? nodeId);
    Task<Node> GetNodeByIdAsync(Guid Id);
    Task<ServerResponse> CreateLawNodes(List<TextLaw> texts, Guid LawId, Guid CreatedBy);
    Task<ServerResponse> UpdateNodeContent([FromBody] NodeContentVm nodeContent, Guid UserId);
    Task<ServerResponse> SetPhaseNodes(Guid LawId, Guid DestinationPhase, Guid LastModifiedBy, int Order);
    Task<ServerResponse> CloneNodes(Guid LawId, Guid CurentPhase, Guid DestinationPhase, string Statu);
    Task<FlatNode[]> GetLawSections(Guid Lawid, Guid PhaseLawId, bool IsPrinVotingFile);
    Task<ServerResponse> cloneSectionWithChildrens(Guid sectionId, Guid destinationPhase);
    Task<ServerResponse> SetNewContent(UpdateNodeContent Node);
    Task<ServerResponse> GetNodeByTypeByNumberByBis(Guid ParentNodeId, Guid TypeId, int Number, int Bis);
}

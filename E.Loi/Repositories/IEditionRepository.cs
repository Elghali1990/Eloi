namespace E.Loi.Repositories;

public interface IEditionRepository
{
    Task<byte[]> PrintTeamAmendments(SetAmendData data);
    Task<byte[]> PrintNode(Guid NodeId);
    Task<byte[]> PrintEditorContent(EditorContent editorContent);
    Task<byte[]> PrintTextLaw(Guid LawId, Guid PhaseLawId, string outType);
    Task<byte[]> GenerateVotingFile(Guid LawId, Guid SectionId, string outType);
    Task<byte[]> PrintVotingFileForPresident(Guid LawId, Guid SectionId, string outType);
    Task<byte[]> printVotingFileCommission(Guid LawId, Guid PhaseLawId, string outType, bool includeAmendmentRatraper);
    Task<byte[]> printVoteAmendmentsResult(Guid LawId, Guid PhaseLawId, string outType);
}

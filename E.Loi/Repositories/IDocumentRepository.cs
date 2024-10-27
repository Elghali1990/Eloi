namespace E.Loi.Repositories;

public interface IDocumentRepository
{
    Task<ServerResponse> InsertDocument(List<DocumentVm> documents);
    Task<List<DocumentVm>> GetLawDocumentsAsync(Guid LawId);
    Task<DocumentVm> GetDocumentAsync(Guid LawId, string Type);
    Task<DocumentVm> GetDocumentByIdAsync(Guid docId);
    Task<ServerResponse> DeleteDocumentAsync(Guid Id);
}

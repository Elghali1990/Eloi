namespace E.Loi.Services.Repositories.Interfaces;

public interface IDocumentRepository : IBaseRepository<Document>
{
    Task<ServerResponse> AddDocumentAsync(List<DocumentVm> documents);
    Task<ServerResponse> DeleteDocumentAsync(Guid Id);
    Task<DocumentVm> GetDocumentAsync(Guid LawId, string Type);
    Task<List<DocumentVm>> GetLawDocumentsAsync(Guid LawId);
}

namespace E.Loi.Services.Repositories;

public class DocumentRepository : BaseRepository<Document>, IDocumentRepository
{
    public DocumentRepository(ILogger logger, LawDbContext db) : base(logger, db)
    {
    }

    public async Task<ServerResponse> AddDocumentAsync(List<DocumentVm> documents)
    {
        try
        {
            bool result = false;
            foreach (var document in documents)
            {
                var doc = new Document()
                {
                    LawId = document.LawId,
                    Type = document.Type,
                    Path = document.Path,
                    DocumentName = document.DocumentName,
                };
                result = await CreateAsync(doc);
            }
            return new ServerResponse(result, result ? "Success" : "Fail");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"${nameof(AddDocumentAsync)} function error", nameof(DocumentRepository));
            throw;
        }
    }

    public async Task<ServerResponse> DeleteDocumentAsync(Guid Id)
    {
        try
        {
            var document = await _dbSet.FirstOrDefaultAsync(x => x.Id == Id);
            if (document != null)
            {
                _dbSet.Remove(document);
                var response = await _db.SaveChangesAsync() > 0;
                return new ServerResponse(response, response ? "Success" : "Fiald");
            }
            return new ServerResponse(false, $"Document where Id : {Id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"${nameof(DeleteDocumentAsync)} function error", nameof(DocumentRepository));
            throw;
        }
    }

    public async Task<DocumentVm> GetDocumentAsync(Guid LawId, string Type)
    {
        try
        {
            var document = await findAsync(doc => doc.LawId == LawId && doc.Type == Type);
            if (document is null)
                return null!;
            return new DocumentVm() { Type = document.Type!, Path = document.Path, LawId = document.LawId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"${nameof(GetDocumentAsync)} function error", nameof(DocumentRepository));
            throw;
        }
    }

    public async Task<List<DocumentVm>> GetLawDocumentsAsync(Guid LawId)
    {
        try
        {
            var documents = await _dbSet
                                  .AsNoTracking()
                                  .AsSingleQuery()
                                  .Where(doc => doc.LawId == LawId)
                                  .Select(d => new DocumentVm() { Id = d.Id, DocumentName = d.DocumentName!, Type = d.Type! })
                                  .ToListAsync();
            return documents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"${nameof(GetLawDocumentsAsync)} function error", nameof(DocumentRepository));
            throw;
        }
    }
}

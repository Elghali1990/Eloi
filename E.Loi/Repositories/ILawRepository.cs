namespace E.Loi.Repositories;
public interface ILawRepository
{
    Task<List<LawVm>> GetAllLawsAsync();
    Task<Law> getLawByYearAsync(int year);
    Task<List<LawListDto>> getAllLawsWithPhases();
    Task<LawInfo> GetLawInfoAsync(Guid LawId);
    Task<List<LawVm>> GetLawsByCategoryAsync(string category);
    Task<List<LawVm>> GetPlfLawAsync();
    Task<Law> GetByIdAsync(Guid LawId);
    Task<List<LawVm>> GetCGILawAsync();
    Task<List<LawVm>> GetAllLawsTeamAsync(string actionName);
    Task<Law> AddLawAsync(EditLawVm model);
    Task<List<LawVm>> GetPreparationLawsAsync(List<Guid> TeamsId);
    Task<ServerResponse> SetLawStatuAsync(LawStatuVm statuVm);
    Task<ServerResponse> GetLawStatuAsync(Guid LawId);
    Task<ServerResponse> SetLawInfo(LawInfo lawInfo);
    Task<ServerResponse> DeleteLawAsync(Guid LawId, Guid LastModifiedBy);
    Task<List<TextLaw>> GetTextLawFromFile(DocumentTexteLoiVM model);
    Task<ServerResponse> SetPhaseLawAsync(Guid LawId, Guid PhaseLawId, Guid LastModifiedBy);
    Task<List<LawVm>> GetAllLawsCommissionAsync(Guid IdCommission);
    Task<List<LawVm>> GetLawsForDirector(List<Guid> EntitiesId, Guid userId);
    Task<List<LawVm>> GetLawsForCommission(List<Guid> EntitiesId, Guid userId);
    Task<List<LawVm>> GetLawsForSession(List<Guid>? EntitiesId, Guid userId);
    Task<List<LawVm>> GetLawsForlegislation();
    Task<List<LawDetail>> GetLawsByCommissionId(Guid commissionId);
    Task<List<LawDetail>> GetLawsByTeamId(Guid teamId);
    Task<List<LawDetail>> GetLawByIds(List<Guid> Ids);
    Task<List<LawVm>> GetLawsToPrint(Guid ComId);
    Task<List<LawVm>> getLawsForReadTwo(Guid phaseLawId);
    Task<List<LawVm>> getAllLawsForAll();
    Task<ServerResponse> setDateIhalaLaw(LawDate lawDate);
    Task<List<LawVm>> GetAllPreparationLawsAsync(Guid PhaseId, Guid TeamId);
    Task<ServerResponse> CheckLawExiste(string Number, int Year);
    Task<List<LawVm>> getAllReferences();
}

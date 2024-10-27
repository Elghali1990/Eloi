namespace E.Loi.Services.Repositories.Interfaces;

public interface ILawRepository : IBaseRepository<Law>
{
    Task<Law> AddLawAsync(EditLawVm model);
    Task<ServerResponse> CreateLawAsync(LawDto lawDto);
    Task<List<LawListDto>> GetAllLawsWithPhases();
    Task<Law> getLawByYearAsync(int year);
    Task<LawInfo> GetLawInfoAsync(Guid LawId);
    Task<ServerResponse> DeleteLawAsync(Guid LawId, Guid LastModifiedBy);
    Task<ServerResponse> SetLawInfoAsync(LawInfo lawInfo);
    Task<ServerResponse> SetPhaseLawAsync(Guid LawId, Guid PhaseLawId, Guid LastModifiedBy);
    Task<ServerResponse> SetLawStatuAsync(LawStatuVm statuVm);
    Task<ServerResponse> GetLawStatuAsync(Guid LawId);
    Task<List<LawDetail>> GetLawByIdCommissionAsync(Guid IdCom);
    Task<List<LawVm>> GetPlfLawAsync();
    Task<List<LawVm>> GetLawsByCategoryAsync(string category);
    Task<List<LawVm>> GetCGILawAsync();
    Task<List<LawVm>> GetAllLawsAsync();
    Task<List<LawVm>> GetAllLawsTeamAsync(List<Guid> PhaseIds);
    Task<List<LawVm>> GetPreparationLawsAsync(List<Guid> TeamsId);
    Task<List<LawVm>> GetAllLawsCommitionAsync(List<Ids> ids, Guid IdCommission);
    Task<List<LawVm>> GetLawsByIdEnititesAsync(List<Guid> phasesId, List<Guid>? EntitiesId, Guid UserId);
    Task<List<LawVm>> GetLawsForlegislation();
    Task<List<LawDetail>> GetLawsByTeamId(Guid teamId);
    Task<List<LawDetail>> GetLawByIds(List<Guid> Ids);
    Task<List<LawVm>> GetLawsToPrint(Guid ComId);
    Task<List<LawVm>> GetLawsForReadTwo(Guid StatuId);
    Task<List<LawVm>> GetAllLawsForAll(List<Guid> PhaseIds);
    Task<ServerResponse> setDateIhalaLaw(LawDate lawDate);
    Task<List<LawVm>> GetAllPreparationLawsAsync(Guid PhaseId, Guid TeamId);
    Task<ServerResponse> CheckLawExiste(string Number, int Year);
    Task<List<LawVm>> getAllReferences();
}

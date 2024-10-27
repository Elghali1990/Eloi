
namespace E.Loi.Components.Layout;

public partial class Phases
{
    /*
     * === Global Variables For Component ===
    */
    [Parameter] public List<string> PhaseIds { get; set; } = new();
    [Parameter] public EventCallback<List<string>> EventTogglePhase { get; set; }
    List<PhaseVm> phases = new();
    //private Phase curentPhase = new();

    /*
     * === On Component Set Parametres ===
    */
    protected override void OnParametersSet()
    {
        //if (!string.IsNullOrEmpty(PhaseId))
        //{
        //    curentPhase = await _phaseRepository.getPhaseById(Guid.Parse(PhaseId));
        //}
        ChangeActivePhase();
        // return base.OnParametersSetAsync();
    }

    protected override void OnInitialized() => ChangeActivePhase();

    /*
   * === Change Toggle Between Phases ===
  */
    private async void TogglePhase(string PhaseId_)
    {
        PhaseIds = phases.Where(p => p.Id == PhaseId_).Select(p => p.Id).ToList();
        await EventTogglePhase.InvokeAsync(PhaseIds);
    }

    /*
     * === Change Active Phase ===
    */
    private void ChangeActivePhase()
    {
        phases = _readJsonFileService.GetPhasesFromJsonFile();
        foreach (var phase in phases)
        {
            if (PhaseIds.Any(p => p.ToLower().Trim() == phase.Id.ToLower().Trim()))
                phase.IsActive = "active-space";
        }
    }
}

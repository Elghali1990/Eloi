
namespace E.Loi.Components.Layout;

public partial class PhasesConsultation
{
    /*
     * === Global Variables For Component ===
    */
    [Parameter] public string PhaseId { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> EventTogglePhase { get; set; }
    List<PhaseVm> phases = new();

    /*
     * === On Component Set Parametres ===
    */
    protected override void OnParametersSet()
    {
        ChangeActivePhase();
    }


    protected override void OnInitialized() => ChangeActivePhase();

    /*
   * === Change Toggle Between Phases ===
  */
    private async void TogglePhase(string PhaseId_)
    {
        PhaseId = PhaseId_;
        await EventTogglePhase.InvokeAsync(PhaseId);
    }

    /*
     * === Change Active Phase ===
    */
    private void ChangeActivePhase()
    {
        phases = _readJsonFileService.GetPhasesConsultationFromJsonFile();
        foreach (var phase in phases)
        {
            if (phase.Id.ToLower() == PhaseId.ToLower())
                phase.IsActive = "active-space";
        }
    }
}

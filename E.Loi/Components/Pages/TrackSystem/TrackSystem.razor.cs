
namespace E.Loi.Components.Pages.TrackSystem;

public partial class TrackSystem
{
    private List<LawListDto> laws = new();
    private List<PhaseDto> phases = new();
    private List<int> phaseOrders = new() { 2, 3, 6, 7 };
    private Guid PhaseId = Guid.Empty, LawId = Guid.Empty;
    private List<CountAmendmentDto> amendments = new();
    private bool processing = false;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            laws = await _lawRepository.getAllLawsWithPhases();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${nameof(OnInitializedAsync)}", nameof(TrackSystem));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }


    private void selectedLaw(ChangeEventArgs e)
    {
        phases.Clear();
        if (e.Value?.ToString() != string.Empty)
        {
            LawId = Guid.Parse(e.Value?.ToString());
            phases = laws.FirstOrDefault(law => law.Id == LawId).Phases.Where(p => phaseOrders.Any(order => order == p.Order)).ToList();
        }
    }

    private void setPhaseId(ChangeEventArgs e)
    {
        if (e.Value?.ToString() != string.Empty)
        {
            PhaseId = Guid.Parse(e.Value?.ToString());
        }
    }

    private async Task counteAmendmentsByTeam()
    {
        try
        {
            amendments.Clear();
            processing = true;
            amendments = await _amendmentRepository.CountAmendmentsByTeamAndLaw(LawId, PhaseId);
            processing = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on ${nameof(counteAmendmentsByTeam)}", nameof(TrackSystem));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }
}

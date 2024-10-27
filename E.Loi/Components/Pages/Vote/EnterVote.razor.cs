namespace E.Loi.Components.Pages.Vote;

public partial class EnterVote
{
    /*
    * === Globals Variables For Component ===
    */
    [SupplyParameterFromForm]
    private VoteVm vote { get; set; } = new();
    private bool IsProcessing = false;
    [Parameter]
    public List<Guid> Ids { get; set; } = new();
    [Parameter]
    public bool VoteNode { get; set; } = false;
    [Parameter]
    public bool VoteAmendment { get; set; } = false;
    [Parameter]
    public EventCallback<bool> OnVote { get; set; }

    /*
    * === Close Modal And Reset Form ===
    */
    public async Task CloseModal()
    {
        ResetForm();
        await jsRuntime.InvokeVoidAsync("HideModal", "ModalVote");
    }

    /*
    * === Reset Form On Initial State ===
    */
    private void ResetForm()
    {
        vote.Against = vote.Neutral = vote.InFavor = 0;
        vote.Result = vote.Observation = string.Empty;
    }

    protected async Task SetVoteResult()
    {
        try
        {
            IsProcessing = true;
            if (Ids.Count > 0)
            {
                vote.Ids = Ids;
                vote.UserId = stateContainerService.user!.Id;
                bool success = false;
                if (VoteNode)
                {
                    success = (await setVote(vote, true)).Flag;
                }
                else
                {
                    success = (await setVote(vote, false)).Flag;
                }
                if (success)
                    toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                else
                    toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                ResetForm();
                IsProcessing = false;
                await jsRuntime.InvokeVoidAsync("HideModal", "ModalVote");
                await OnVote.InvokeAsync(success);
            }
        }
        catch (Exception ex)
        {
            IsProcessing = false;
            _logger.LogError(ex.Message, "Error on SetVoteResult", nameof(Vote));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    async Task<ServerResponse> setVote(VoteVm voteVm, bool voteType)
    {
        if (voteType)
            return await _voteRepository.InsertVote(vote);
        else
            return await _voteAmendmentRepository.InsertVote(vote);
    }
}

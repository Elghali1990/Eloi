namespace E.Loi.Components.Pages.Amendments
{
    public partial class UploadAmendment
    {
        /*
         * === ===
        */
        [Parameter] public string LawId { get; set; } = string.Empty;
        [Parameter] public string PhaseId { get; set; } = string.Empty;
        [Parameter] public bool IsDisplay { get; set; }
        AmendmentsListVm[]? amendmentsListVms;
        AmendmentsListVm[]? submittedAmendmentList;
        bool isLoad = false, isSubmit = false;
        private Guid AmendId = Guid.Empty;
        IEnumerable<AmendmentsListVm> selectedAmendments = new List<AmendmentsListVm>();


        /*
         * === On Component Set Parametre ===
        */
        protected override async Task OnParametersSetAsync()
        {
            try
            {
                await LoadAmendment();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error on OnInitializedAsync", nameof(UploadAmendment));
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }

        /*
         * === On Initialize Components ===
        */
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadAmendment();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error on OnInitializedAsync", nameof(UploadAmendment));
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }

        }

        /*
         * === Load Amendments Where Vote Result Is Unacceptable Or Pulled ===
        */
        async Task LoadAmendment()
        {
            if (!string.IsNullOrEmpty(LawId) && !string.IsNullOrEmpty(PhaseId))
            {
                string OldPhase = string.Empty;
                if (_phaseOptions.Value.PHASE_SEANCE_PLENIERE_1.ToLower().Equals(PhaseId.ToLower()))
                    OldPhase = _phaseOptions.Value.PHASE_COMMISSION_1;
                if (_phaseOptions.Value.PHASE_SEANCE_PLENIERE_2.ToLower().Equals(PhaseId.ToLower()))
                    OldPhase = _phaseOptions.Value.PHASE_COMMISSION_1;
                if (OldPhase != string.Empty)
                {
                    var result = await _amendmentRepository.GetLiftAmendments(stateContainerService.user!.TeamsDtos.Select(x => x.Id).ToList(), Guid.Parse(LawId), Guid.Parse(_phaseOptions.Value.PHASE_COMMISSION_1));
                    if (result != null) amendmentsListVms = result.ToArray();
                }
                isLoad = true;
            }
        }

        /*
         * === Submit Amendments ===
        */
        protected async Task SubmitAmendments()
        {
            try
            {
                isSubmit = !isSubmit;
                if (selectedAmendments is not null)
                {
                    var amendments = new CloneAmendmentsVm()
                    {
                        PhaseId = Guid.Parse(PhaseId),
                        UserId = stateContainerService.user!.Id,
                        Ids = selectedAmendments.Select(a => a.Id).ToList()
                    };
                    var response = await _amendmentRepository.CloneAmendmentsAsync(amendments);
                    if (response.Flag)
                    {
                        submittedAmendmentList = selectedAmendments.ToArray();
                        List<AmendmentsListVm> amendmentsListVms_ = new();
                        foreach (var amd in amendmentsListVms!)
                        {
                            if (!amendments.Ids.Contains(amd.Id))
                            {
                                amendmentsListVms_.Add(amd);
                            }
                        }
                        amendmentsListVms = amendmentsListVms_.ToArray();
                        toastService.ShowSuccess(Constants.MessageUploadAmendments, settings => { settings.DisableTimeout = settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                        await _traceService.insertTrace(new Trace { Operation = "Upload Amendments ", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
                    }
                    else
                    {
                        toastService.ShowError(Constants.MessageError, settings => { settings.DisableTimeout = settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                    }
                }
                isSubmit = !isSubmit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error on OnInitializedAsync", nameof(UploadAmendment));
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
        }

        /*
         * === Amendment Detail === 
        */
        protected async Task handleGetAmendmentDetails(Guid Id)
        {
            AmendId = Id;
            await jsRuntime.InvokeVoidAsync("ShowModal", "ModalDetail");
        }

    }
}

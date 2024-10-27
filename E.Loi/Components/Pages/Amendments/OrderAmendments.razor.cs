namespace E.Loi.Components.Pages.Amendments;

public partial class OrderAmendments
{
    /*
     * === Globale Page Variables ===
    */
    [Parameter] public string? LawId { get; set; }
    [Parameter] public string? PhaseId { get; set; }
    AmendmentsListVm[] amendmentsListVms;
    int TotaleRows = 0, loupCounter = 0, Order = 0, newOrder = 0;
    bool IsLoad = false, IsLoadAmendments = false, IsProcessing = false;
    List<FlatNode> parentsNode = new();
    List<NodeVm> nodes = new();
    AmendmentDetails amendmentDetails = new();
    Guid amendmentId = Guid.Empty;

    /*
     * === OnInitialized Component ===
    */
    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsLoad = true;
            List<AmendmentsListVm> list = new();
            if (!string.IsNullOrEmpty(LawId) && !string.IsNullOrEmpty(PhaseId))
            {
                var items = await _amendmentRepository.GetAllSubmitedAmendments(Guid.Parse(LawId), Guid.Parse(PhaseId));
                int counter = 0;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        counter++;
                        list.Add(new AmendmentsListVm
                        {
                            Id = item.Id,
                            Number = item.Number,
                            Order = counter,
                            NodeTitle = item.NodeTitle,
                            Title = item.Title,
                            AmendmentType = item.AmendmentType,
                            AmendmentIntent = item.AmendmentIntent,
                            AmendmentsStatus = item.AmendmentsStatus,
                            ParentNodeTitle = item.ParentNodeTitle,
                            Team = item.Team,
                            NumberSystem = item.NumberSystem,
                            CreationDate = item.CreationDate,
                            SubmitedDate = item.SubmitedDate
                        });
                    }
                    amendmentsListVms = list.ToArray();
                    TotaleRows = amendmentsListVms.Length / 20;
                    if (TotaleRows < 1)
                        TotaleRows = 1;
                }
            }
            IsLoad = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error on {nameof(OnInitializedAsync)}", nameof(OrderAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Incress Order Amendments ===
    */
    protected void IncreseOrder(Guid Id)
    {
        try
        {
            var amendToIncrese = amendmentsListVms!.FirstOrDefault(am => am.Id == Id);
            int IndexElementToIncress = Array.FindIndex(amendmentsListVms!, row => row.Id == Id);
            int IndexElementToDecress = Array.FindIndex(amendmentsListVms!, row => row.Order == (amendToIncrese!.Order + 1));
            amendmentsListVms![IndexElementToIncress].Order = amendmentsListVms[IndexElementToIncress].Order + 1;
            amendmentsListVms![IndexElementToIncress].IsOrderChanged = true;
            amendmentsListVms[IndexElementToDecress].Order = amendmentsListVms[IndexElementToDecress].Order - 1;
            amendmentsListVms = amendmentsListVms.OrderBy(a => a.Order).ToArray();
            loupCounter = 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {IncreseOrder}", nameof(OrderAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Decress Order Amendments ===
    */
    private void DecreseOrder(Guid Id)
    {
        try
        {
            var amendToDecress = amendmentsListVms!.FirstOrDefault(am => am.Id == Id);
            int IndexElementToDecress = Array.FindIndex(amendmentsListVms!, row => row.Order == (amendToDecress!.Order));
            int IndexElementToIncress = Array.FindIndex(amendmentsListVms!, row => row.Order == (amendToDecress!.Order - 1));
            amendmentsListVms![IndexElementToDecress].Order = amendmentsListVms[IndexElementToDecress].Order - 1;
            amendmentsListVms![IndexElementToDecress].IsOrderChanged = true;
            amendmentsListVms![IndexElementToIncress].Order = amendmentsListVms[IndexElementToIncress].Order + 1;
            amendmentsListVms = amendmentsListVms.OrderBy(a => a.Order).ToArray();
            loupCounter = 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {IncreseOrder}", nameof(OrderAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

    /*
     * === Handle Order Amendments ===
    */
    private async Task HandleSetOrdersAmendments()
    {
        try
        {
            SetAmendmentOrder model = new();
            IsProcessing = true;
            model.AmendmentOrders = amendmentsListVms!.Select(am => new AmendmentOrder { Id = am.Id, Order = am.Order }).ToList();
            model.LastModifiedBy = stateContainerService.user!.Id;
            var response = await _amendmentRepository.SetAmendmentsOrders(model);
            if (response.Flag)
            {
                toastService.ShowSuccess(Constants.SuccessOperation, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
                await _traceService.insertTrace(new Trace { Operation = "Order Amendments ", DateOperation = DateTime.UtcNow, UserId = stateContainerService.user.Id });
            }
            else
            {
                toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            IsProcessing = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, $"Error On {HandleSetOrdersAmendments}", nameof(OrderAmendments));
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            toastService.ShowError(Constants.MessageError, settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }

    }

    /*
     * === Show Amendment Details ===
    */
    protected async Task handleGetAmendmentDetails(Guid Id)
    {
        amendmentId = Id;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModalDetail");
    }

    /*
     * === Show Show Modal Order ===
    */
    private async Task ShowModal(int order)
    {
        Order = order;
        await jsRuntime.InvokeVoidAsync("ShowModal", "ModalOrder");
    }

    /*
     * === private async Task setAmendmentOrder ===
    */
    private void setAmendmentOrder()
    {
        int maxOrder = amendmentsListVms!.Max(x => x.Order);
        if (newOrder > 0)
        {
            if (newOrder > maxOrder)
            {
                toastService.ShowError("رقم الترتيبي الذي أدخلتم غير صحيح", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
            }
            else
            {
                var amedToMove_1 = amendmentsListVms!.FirstOrDefault(a => a.Order == Order);
                var amedToMove_2 = amendmentsListVms!.FirstOrDefault(a => a.Order == newOrder);
                int Index_AmedToMove_1 = Array.FindIndex(amendmentsListVms!, row => row.Order == amedToMove_1!.Order);
                int Index_AmedToMove_2 = Array.FindIndex(amendmentsListVms!, row => row.Order == amedToMove_2!.Order);
                amendmentsListVms![Index_AmedToMove_1].Order = newOrder;
                amendmentsListVms![Index_AmedToMove_1].IsOrderChanged = true;
                amendmentsListVms![Index_AmedToMove_2].Order = Order;
                amendmentsListVms![Index_AmedToMove_2].IsOrderChanged = true;
                amendmentsListVms = amendmentsListVms.OrderBy(a => a.Order).ToArray();
                newOrder = loupCounter = 0;
                Task.Run(() => jsRuntime.InvokeVoidAsync("HideModal", "ModalOrder"));
            }
        }
        else
        {
            toastService.ShowError("المرجو إدخال الرقم الترتيبي.", settings => { settings.ShowCloseButton = false; settings.Position = ToastPosition.BottomCenter; });
        }
    }

}

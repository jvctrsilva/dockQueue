using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using DockQueue.Client.Utils;

namespace DockQueue.Client.Shared;

public class BasePage : ComponentBase
{
    protected bool isLoading = true;
    protected DateTime DataIni { get; set; }
    protected DateTime DataFim { get; set; }


#pragma warning disable CS8618
    [Inject] protected NavigationManager navManager { get; set; }
    [Inject] protected SweetAlertService Swal       { get; set; }
#pragma warning restore CS8618

    public async void Refresh()
    {
        if (isLoading)
            return;

        isLoading = true;
        await Task.Delay(20);
        await InvokeAsync(StateHasChanged);

        isLoading = false;
        await Task.Delay(20);
        await InvokeAsync(StateHasChanged);
    }

    public async Task HandleErrorAsync(Exception ex, System.Security.Claims.ClaimsPrincipal user, string? mensagem = null)
    {
        await SWA.ExibirAlerta(Swal, "Erro", mensagem ?? ex.Message);
        //await LogService.LogAsync(ex, user);
    }
}
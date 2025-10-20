using CurrieTechnologies.Razor.SweetAlert2;

namespace DockQueue.Utils;

public static class SWA
{
    public static async Task ExibirAlerta(SweetAlertService Swal, string tipo, string mensagem)
    {
        switch (tipo)
        {
            case "Aviso":
                await Aviso(Swal, mensagem);
                break;
            case "Erro":
                await Erro(Swal, mensagem);
                break;
            case "Sucesso":
                await Sucesso(Swal, mensagem);
                break;
            case "Informacao":
                await Informacao(Swal, mensagem);
                break;
            default:
                await Aviso(Swal, mensagem);
                break;
        }
    }


    public static async Task Sucesso(SweetAlertService Swal, string texto)
    {
        await Swal.FireAsync(new SweetAlertOptions
        {
            Title = "Sucesso",
            Text = texto,
            Icon = SweetAlertIcon.Success,
            ShowCancelButton = false,
            ConfirmButtonText = "OK"
        });
    }

    public static async Task Erro(SweetAlertService Swal, string texto)
    {
        await Swal.FireAsync(new SweetAlertOptions
        {
            Title = "Erro",
            Text = texto,
            Icon = SweetAlertIcon.Error,
            ShowCancelButton = false,
            ConfirmButtonText = "OK"
        });
    }

    public static async Task Aviso(SweetAlertService Swal, string texto)
    {
        await Swal.FireAsync(new SweetAlertOptions
        {
            Title = "Aviso",
            Text = texto,
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = false,
            ConfirmButtonText = "OK"
        });
    }
    public static async Task Informacao(SweetAlertService Swal, string texto)
    {
        await Swal.FireAsync(new SweetAlertOptions
        {
            Title = "Atenção",
            Text = texto,
            Icon = SweetAlertIcon.Info,
            ShowCancelButton = false,
            ConfirmButtonText = "OK"
        });
    }
    public static async Task MensagemPergunta(SweetAlertService Swal, string texto)
    {
        await Swal.FireAsync(new SweetAlertOptions
        {
            Title = "Pergunta",
            Text = texto,
            Icon = SweetAlertIcon.Question,
            ShowCancelButton = false,
            ConfirmButtonText = "OK"
        });
    }
    public static async Task MensagemCustom(SweetAlertService Swal, string titulo, string texto, SweetAlertIcon icone, string textoBotao, string corFundo)
    {
        await Swal.FireAsync(new SweetAlertOptions
        {
            Title = titulo,
            Text = texto,
            Icon = icone,
            ShowCancelButton = false,
            ConfirmButtonText = textoBotao,
            Background = corFundo
        });
    }
}
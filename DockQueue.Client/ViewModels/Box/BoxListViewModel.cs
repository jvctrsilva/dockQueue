using System.ComponentModel.DataAnnotations;
using DockQueue.Application.DTOs;
using DockQueue.Client.Services;

namespace DockQueue.Client.ViewModels.Box;

/// <summary>
/// Controla o estado da tela Boxes.
/// Responsável por carregar, adicionar e remover boxes.
/// </summary>
public class BoxListViewModel
{
    private readonly BoxService _service;
    public BoxListViewModel(BoxService service) => _service = service;
    public List<BoxDto> Items { get; private set; } = new();
    public bool IsLoading { get; private set; }
    public string? Error { get; private set; }

    /// Carrega todos os boxes da API.
    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            Items = await _service.GetAllAsync();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally { IsLoading = false; }
    }

    /// Remove um box e atualiza a lista local.
    public async Task RemoveAsync(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            var idx = Items.FindIndex(x => x.Id == id);
            if (idx >= 0) Items.RemoveAt(idx);
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }
}

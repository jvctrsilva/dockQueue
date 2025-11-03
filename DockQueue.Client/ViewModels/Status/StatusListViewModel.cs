using DockQueue.Application.DTOs;
using DockQueue.Client.Services;

namespace DockQueue.Client.ViewModels;

public class StatusListViewModel
{
    private readonly StatusesService _service;
    public StatusListViewModel(StatusesService service) => _service = service;

    public List<StatusDto> Items { get; private set; } = new();
    public bool IsLoading { get; private set; }
    public string? Error { get; private set; }

    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            Error = null;
            Items = await _service.GetAllAsync();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally { IsLoading = false; }
    }

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

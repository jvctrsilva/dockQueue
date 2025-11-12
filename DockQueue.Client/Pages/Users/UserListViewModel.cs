using DockQueue.Application.DTOs;
using DockQueue.Client.Services;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DockQueue.Client.Pages.Users;
public class UserListViewModel
{
    private readonly UserService _service;
    public UserListViewModel(UserService service) => _service = service;
    public List<UserDto> Users { get; private set; } = new();
    public bool IsLoading { get; private set; }
    public string? Error { get; private set; }

    public async Task LoadAsync()
    {
        try
        {
            IsLoading = true;
            Users = await _service.GetAllAsync();
        }
        catch (Exception ex)
        {
            Error = $"Erro ao carregar pessoas: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
    public async Task RemoveAsync(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            var idx = Users.FindIndex(x => x.Id == id);
            if (idx >= 0) Users.RemoveAt(idx);
        }
        catch (Exception ex)
        {
            Error = $"Erro ao remover: {ex.Message}";
        }
    }
}

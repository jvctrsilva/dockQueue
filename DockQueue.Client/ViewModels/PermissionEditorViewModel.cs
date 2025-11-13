using DockQueue.Application.DTOs;
using DockQueue.Application.DTOs.Permissions;
using DockQueue.Client.Services;
using DockQueue.Domain.ValueObjects;

public class PermissionEditorViewModel
{
    private readonly IPermissionsApi _api;
    public PermissionEditorViewModel(IPermissionsApi api) => _api = api;

    public int UserId { get; private set; }
    public bool IsLoading { get; private set; }
    public string? Error { get; private set; }

    public IReadOnlyList<StatusDto> AllStatuses { get; private set; } = Array.Empty<StatusDto>();
    public IReadOnlyList<BoxDto> AllBoxes { get; private set; } = Array.Empty<BoxDto>();

    public HashSet<int> SelectedStatusIds { get; private set; } = new();
    public HashSet<int> SelectedBoxIds { get; private set; } = new();
    public Screen AllowedScreens { get; set; } = Screen.None;

    public async Task LoadAsync(int userId, CancellationToken ct = default)
    {
        try
        {
            IsLoading = true; Error = null; UserId = userId;

            var payload = await _api.GetScreenDataAsync(userId, ct);

            if (payload is null) { Error = "Não encontrado."; return; }

            AllStatuses = payload.AllStatuses ?? new List<StatusDto>();
            AllBoxes = payload.AllBoxes ?? new List<BoxDto>();

            var perms = payload.UserPermissions ?? new OperatorPermissionsDto { UserId = userId };
            SelectedStatusIds = new(perms.AllowedStatusIds ?? new());
            SelectedBoxIds = new(perms.AllowedBoxIds ?? new());
            AllowedScreens = perms.AllowedScreens;
        }
        catch (Exception ex) { Error = $"Erro ao carregar permissões: {ex.Message}"; }
        finally { IsLoading = false; }
    }

    public void ToggleStatus(int id, bool on) => _ = on ? SelectedStatusIds.Add(id) : SelectedStatusIds.Remove(id);
    public void ToggleBox(int id, bool on) => _ = on ? SelectedBoxIds.Add(id) : SelectedBoxIds.Remove(id);
    public void ToggleScreen(Screen flag, bool on) => AllowedScreens = on ? (AllowedScreens | flag) : (AllowedScreens & ~flag);

    public async Task<bool> SaveAsync()
    {
        try
        {
            IsLoading = true; Error = null;
            var dto = new UpdateOperatorPermissionsDto
            {
                AllowedStatusIds = SelectedStatusIds.ToList(),
                AllowedBoxIds = SelectedBoxIds.ToList(),
                AllowedScreens = AllowedScreens
            };
            var ok = await _api.UpdateAsync(UserId, dto);
            if (!ok) Error = "Falha ao salvar.";
            return ok;
        }
        catch (Exception ex) { Error = $"Erro ao salvar permissões: {ex.Message}"; return false; }
        finally { IsLoading = false; }
    }
}

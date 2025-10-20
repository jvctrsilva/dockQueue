namespace DockQueue.Services.UI;

public class UserPermissionsService
{
    private static Dictionary<Guid, UserPermissions>? _store;

    public async Task<List<DriverStatusItem>> GetAllDriverStatusesAsync()
    {
        await Task.Delay(50);
        return new List<DriverStatusItem>
        {
            new DriverStatusItem { Key = "aguardando", Name = "Aguardando" },
            new DriverStatusItem { Key = "em_atendimento", Name = "Em atendimento" },
            new DriverStatusItem { Key = "concluido", Name = "Concluído" },
            new DriverStatusItem { Key = "cancelado", Name = "Cancelado" },
        };
    }

    public async Task<List<ViewItem>> GetAllViewsAsync()
    {
        await Task.Delay(50);
        return new List<ViewItem>
        {
            new ViewItem { Key = "dashboard", Name = "Dashboard" },
            new ViewItem { Key = "filas", Name = "Filas" },
            new ViewItem { Key = "docas", Name = "Docas" },
            new ViewItem { Key = "agendamentos", Name = "Agendamentos" },
            new ViewItem { Key = "relatorios", Name = "Relatórios" },
            new ViewItem { Key = "configuracoes", Name = "Configurações" },
        };
    }

    public async Task<UserPermissions> GetUserPermissionsAsync(Guid userId)
    {
        await Task.Delay(100);
        _store ??= new Dictionary<Guid, UserPermissions>();
        if (!_store.TryGetValue(userId, out var perms))
        {
            perms = CreateDefault(userId);
            _store[userId] = perms;
        }
        return perms;
    }

    public async Task SaveUserPermissionsAsync(UserPermissionsUpdate update)
    {
        await Task.Delay(100);
        _store ??= new Dictionary<Guid, UserPermissions>();
        _store[update.UserId] = new UserPermissions
        {
            StatusKeys = update.StatusKeys.Distinct(StringComparer.OrdinalIgnoreCase).ToList(),
            Boxes = update.Boxes.Distinct().OrderBy(x => x).ToList(),
            ViewKeys = update.ViewKeys.Distinct(StringComparer.OrdinalIgnoreCase).ToList()
        };
    }

    private static UserPermissions CreateDefault(Guid userId)
    {
        return new UserPermissions
        {
            StatusKeys = new List<string> { "aguardando", "em_atendimento" },
            Boxes = new List<int> { 1, 2, 3 },
            ViewKeys = new List<string> { "dashboard", "filas" }
        };
    }
}

public class DriverStatusItem
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class ViewItem
{
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class UserPermissions
{
    public List<string> StatusKeys { get; set; } = new();
    public List<int> Boxes { get; set; } = new();
    public List<string> ViewKeys { get; set; } = new();
}

public class UserPermissionsUpdate : UserPermissions
{
    public Guid UserId { get; set; }
}



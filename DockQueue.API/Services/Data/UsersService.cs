namespace DockQueue.Services.UI;

public class UsersService
{
    private static List<UserListItem>? _cache;

    public async Task<List<UserListItem>> GetUsersAsync()
    {
        await Task.Delay(100);
        return _cache ??= CreateUsers();
    }

    public async Task<UserListItem?> GetUserAsync(Guid id)
    {
        var all = await GetUsersAsync();
        return all.FirstOrDefault(u => u.Id == id);
    }

    private static List<UserListItem> CreateUsers()
    {
        return new List<UserListItem>
        {
            new UserListItem { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Ana Souza", Email = "ana@empresa.com", Active = true },
            new UserListItem { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Bruno Lima", Email = "bruno@empresa.com", Active = true },
            new UserListItem { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Carla Dias", Email = "carla@empresa.com", Active = false },
        };
    }
}

public class UserListItem
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool Active { get; set; }
}



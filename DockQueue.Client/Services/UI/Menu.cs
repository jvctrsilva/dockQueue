namespace DockQueue.Client.Services.UI;

public class MainMenuItems
{
    public string MenuTitle { get; set; }
    public string Title { get; set; }
    public string Path { get; set; }
    public string Icon { get; set; }
    public string Type { get; set; }
    public int RandomNumber { get; set; }
    public string BadgeClass { get; set; }
    public string BadgeValue { get; set; }
    public bool Active { get; set; }
    public bool Selected { get; set; }
    public bool DirChange { get; set; }
    public List<string> Roles { get; set; } // Adicione esta linha
    public MainMenuItems[]? Children { get; set; }

    // Constructor to initialize an instance of MainMenuItems
    public MainMenuItems(string title = "", string path = "", int randomNumber = 0, string icon = "", string type = "", string menuTitle = "", string badgeClass = "", string badgeValue = "", bool active = false, bool selected = false, bool dirChange = false, MainMenuItems[]? children = null, List<string>? roles = null)
    {
        MenuTitle       = menuTitle;
        Title           = title;
        Path            = path;
        RandomNumber    = randomNumber;
        Icon            = icon;
        Type            = type;
        BadgeClass      = badgeClass;
        BadgeValue      = badgeValue;
        Active          = active;
        Selected        = selected;
        DirChange       = dirChange;
        Children        = children;
        Roles           = roles ?? new List<string>(); // Inicializa roles
    }

    // Construtor para inicializar uma instância de MainMenuItems
    public MainMenuItems(MainMenuItems item)
    {
        MenuTitle       = item.MenuTitle;
        Title           = item.Title;
        Path            = item.Path;
        RandomNumber    = item.RandomNumber;
        Icon            = item.Icon;
        Type            = item.Type;
        BadgeClass      = item.BadgeClass;
        BadgeValue      = item.BadgeValue;
        Active          = item.Active;
        Selected        = item.Selected;
        DirChange       = item.DirChange;
        Children        = item.Children;
        Roles           = item.Roles;
    }

    // Construtor sem parâmetros
    public MainMenuItems() { }
}
namespace DockQueue.Services.UI;

public class MenuDataService
{
    private List<MainMenuItems> MenuData = new List<MainMenuItems>()
    {
        new MainMenuItems(),
        new MainMenuItems(
            path: "/home",
            type: "link",
            title: "Home",
            icon: "bx bx-list-square",
            badgeClass: "bg-warning-transparent",
            selected: false,
            active: false,
            dirChange: false
        ),
        new MainMenuItems(
            path: "/boxes",
            type: "link",
            title: "Box",
            icon: "bx bx-list-square",
            badgeClass: "bg-success-transparent",
            selected: false,
            active: false,
            dirChange: false
        ),
        new MainMenuItems(
            path: "/operadores",
            type: "link",
            title: "Operadores",
            icon: "bx bx-list-square",
            badgeClass: "bg-warning-transparent",
            selected: false,
            active: false,
            dirChange: false
        ),
        new MainMenuItems(
            path: "/horario-funcionamento",
            type: "link",
            title: "Funcionamento",
            icon: "bx bx-time",
            badgeClass: "bg-info-transparent",
            selected: false,
            active: false,
            dirChange: false
        ),
        new MainMenuItems(
            path: "/permissoes-usuario",
            type: "link",
            title: "Permissões",
            icon: "bx bx-lock",
            badgeClass: "bg-info-transparent",
            selected: false,
            active: false,
            dirChange: false
        ),
    };

    public List<MainMenuItems> GetMenuData() => MenuData;

}
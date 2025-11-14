using DockQueue.Domain.ValueObjects;

namespace DockQueue.Client.Services.UI;

public class MenuDataService
{
    private List<MainMenuItems> MenuData = new List<MainMenuItems>()
    {
        new MainMenuItems(
            path: "/home",
            type: "link",
            title: "Home",
            icon: "bx bx-list-square",
            badgeClass: "bg-warning-transparent",
            selected: false,
            active: false,
            dirChange: false,
            screenRequired:null 
        ),

        new MainMenuItems(
            path: "/filas",
            type: "link",
            title: "Filas",
            icon: "bx bx-queue",
            badgeClass: "bg-info-transparent",
            selected: false,
            active: false,
            dirChange: false,
            screenRequired: Screen.QueueView // exige a flag StatusView
        ),
        new MainMenuItems(
            path: "/status",
            type: "link",
            title: "Status",
            icon: "bx bx-list-check",
            badgeClass: "bg-info-transparent",
            selected: false,
            active: false,
            dirChange: false,
            screenRequired: Screen.StatusView // exige a flag StatusView
        ),
        new MainMenuItems(
            path: "/boxes",
            type: "link",
            title: "Box",
            icon: "bx bx-box-alt",
            badgeClass: "bg-success-transparent",
            selected: false,
            active: false,
            dirChange: false, 
            screenRequired: Screen.BoxesView
        ),
        new MainMenuItems(
            path: "/usuarios",
            type: "link",
            title: "Operadores",
            icon: "bx bx-user",
            badgeClass: "bg-warning-transparent",
            selected: false,
            active: false,
            dirChange: false,
            screenRequired: Screen.UsersView
        ),
        new MainMenuItems(
            path: "/settings/operating-schedule",
            type: "link",
            title: "Configurações",
            icon: "bx bx-gear",
            badgeClass: "bg-info-transparent",
            selected: false,
            active: false,
            dirChange: false,
            screenRequired: Screen.SettingsView
        ),
        new MainMenuItems(
            path: "/permissoes-usuario",
            type: "link",
            title: "Permissões",
            icon: "bx bx-lock",
            badgeClass: "bg-info-transparent",
            selected: false,
            active: false,
            dirChange: false,
            screenRequired: Screen.PermissionsView
        ),
    };

    public List<MainMenuItems> GetMenuData() => MenuData;

}
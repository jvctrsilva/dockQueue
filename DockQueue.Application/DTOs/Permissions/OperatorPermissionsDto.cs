using DockQueue.Domain.ValueObjects;

namespace DockQueue.Application.DTOs.Permissions
{
    // Retorno para a tela (3 abas + dashboard)
    public class OperatorPermissionsDto
    {
        public int UserId { get; set; }
        public List<int> AllowedStatusIds { get; set; } = new();
        public List<int> AllowedBoxIds { get; set; } = new();
        public Screen AllowedScreens { get; set; } // Flags (exibir no dashboard/toggles)
        public DateTime UpdatedAt { get; set; }
    }
}

using DockQueue.Domain.ValueObjects;

namespace DockQueue.Application.DTOs.Permissions
{
    // Payload enviado pela tela (abas) para atualizar permissões do operador
    public class UpdateOperatorPermissionsDto
    {
        public List<int> AllowedStatusIds { get; set; } = new();
        public List<int> AllowedBoxIds { get; set; } = new();
        public Screen AllowedScreens { get; set; } = Screen.None; // toggles de visualização
    }
}

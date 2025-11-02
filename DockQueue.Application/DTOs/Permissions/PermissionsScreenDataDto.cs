
namespace DockQueue.Application.DTOs.Permissions
{
    public class PermissionsScreenDataDto
    {
        public List<StatusDto> AllStatuses { get; set; } = new();
        public List<BoxDto> AllBoxes { get; set; } = new();
        public OperatorPermissionsDto UserPermissions { get; set; } = new();
    }
}
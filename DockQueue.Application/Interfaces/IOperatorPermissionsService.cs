using DockQueue.Application.DTOs.Permissions;

namespace DockQueue.Application.Interfaces
{
    public interface IOperatorPermissionsService
    {
        Task<OperatorPermissionsDto?> GetByUserIdAsync(int userId);
        Task<OperatorPermissionsDto> UpsertAsync(int userId, UpdateOperatorPermissionsDto dto);
    }
}

using DockQueue.Application.DTOs.Permissions;

namespace DockQueue.Application.Interfaces
{
    public interface IOperatorPermissionsService
    {
        Task<PermissionsScreenDataDto?> GetScreenDataAsync(int userId, CancellationToken ct);
        Task<OperatorPermissionsDto?> GetByUserIdAsync(int userId, CancellationToken ct);
        Task<OperatorPermissionsDto> UpsertAsync(int userId, UpdateOperatorPermissionsDto dto, CancellationToken ct);
    }
}

using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface IOperatorPermissionsRepository
    {
        Task<OperatorPermissions?> GetByUserIdAsync(int userId, CancellationToken ct);
        Task<OperatorPermissions> UpsertAsync(OperatorPermissions incoming, CancellationToken ct);
    }
}

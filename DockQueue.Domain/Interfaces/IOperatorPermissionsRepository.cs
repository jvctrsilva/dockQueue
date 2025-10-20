using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface IOperatorPermissionsRepository
    {
        Task<OperatorPermissions?> GetByUserIdAsync(int userId);
        Task<OperatorPermissions> UpsertAsync(OperatorPermissions incoming);
    }
}

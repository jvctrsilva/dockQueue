using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface IStatusRepository
    {
        Task<List<Status>> GetAllAsync();
        Task<Status?> GetByCodeAsync(string code);
        Task<Status> GetByIdAsync(int id);
        Task<Status> AddAsync(Status status);
        Task UpdateAsync(Status status);
        Task<bool> ExistsDefaultAsync(int? ignoreId = null);
    }
}
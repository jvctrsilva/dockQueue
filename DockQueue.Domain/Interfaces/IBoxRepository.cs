using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface IBoxRepository
    {
        Task<List<Box>> GetAllAsync(CancellationToken ct = default);
        Task<Box?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Box> AddAsync(Box box, CancellationToken ct = default);
        Task UpdateAsync(Box box, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}

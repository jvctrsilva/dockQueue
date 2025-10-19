using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface IBoxRepository
    {
        Task<List<Box>> GetAllAsync();
        Task<Box?> GetByIdAsync(int id);
        Task<Box> AddAsync(Box box);
        Task UpdateAsync(Box box);
        Task DeleteAsync(int id);
    }
}

using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();
        Task<User> GetByIdAsync(int id);
        Task<User> GetByEmailAsync(string email);
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task Update(User user);
        Task DeleteAsync(int id);
    }
}

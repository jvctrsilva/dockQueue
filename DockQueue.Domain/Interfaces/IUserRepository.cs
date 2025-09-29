using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task <List <User>> GetAllAsync();
        Task <User> GetByIdAsync(int id);
        Task <User> AddAsync(User user);
        Task <User?> GetByEmailAsync(string email);
    }
}

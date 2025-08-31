using DockQueue.Domain;
using DockQueue.Infrastructure;
using DockQueue.Application.Security;
using Microsoft.EntityFrameworkCore;
using DockQueue.Application.Dtos;

namespace DockQueue.Application.Services
{
    public class UserService
    {
        private readonly DockQueueDbContext _db;

        public UserService(DockQueueDbContext db)
        {
            _db = db;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _db.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Number = u.Number,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Number = user.Number,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task <User> CreateUserAsync(User user)
        {
            // Hash da senha antes de salvar
            user.Password = PasswordHasher.Hash(user.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}

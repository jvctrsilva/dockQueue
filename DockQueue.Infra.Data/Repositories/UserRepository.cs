using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DockQueue.Domain.Validation;

namespace DockQueue.Infra.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) { _context = context; }

        public async Task <User?> GetByEmailAsync(string email) => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<List<User>> GetAllAsync() => await _context.Users.ToListAsync();

        public async Task<User> GetByIdAsync(int id) => await _context.Users.FindAsync(id) ?? throw new DomainExceptionValidation.EntityNotFoundException($"Usuário com id {id} não encontrado");

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}

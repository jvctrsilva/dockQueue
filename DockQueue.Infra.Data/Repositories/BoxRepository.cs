using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using DockQueue.Infra.Data.Context;
using DockQueue.Domain.Validation;


namespace DockQueue.Infra.Data.Repositories
{
    public class BoxRepository : IBoxRepository
    {
        private readonly ApplicationDbContext _context;

        public BoxRepository(ApplicationDbContext context) { _context = context; }

        public async Task<List<Box>> GetAllAsync()
        {
            return await _context.Boxes
                                 .Include(b => b.Driver) // carrega motorista junto
                                 .ToListAsync();
        }

        public async Task<Box?> GetByIdAsync(int id)
        {
            return await _context.Boxes
                                 .Include(b => b.Driver)
                                 .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Box> AddAsync(Box box)
        {
            _context.Boxes.Add(box);
            await _context.SaveChangesAsync();
            return box;
        }

        public async Task UpdateAsync(Box box)
        {
            _context.Boxes.Update(box);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var box = await GetByIdAsync(id);
            if (box != null)
            {
                _context.Boxes.Remove(box);
                await _context.SaveChangesAsync();
            }
        }
    }
}

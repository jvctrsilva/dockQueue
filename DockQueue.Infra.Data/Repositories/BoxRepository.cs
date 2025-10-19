using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Domain.Validation;
using DockQueue.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DockQueue.Infra.Data.Repositories
{
    public class BoxRepository : IBoxRepository
    {
        private readonly ApplicationDbContext _ctx;
        public BoxRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<List<Box>> GetAllAsync() =>
            await _ctx.Boxes.AsNoTracking().ToListAsync();

        public async Task<Box?> GetByIdAsync(int id) =>
            await _ctx.Boxes.FindAsync(id);

        public async Task<Box> AddAsync(Box box)
        {
            _ctx.Boxes.Add(box);
            await _ctx.SaveChangesAsync();
            return box;
        }

        public async Task UpdateAsync(Box box)
        {
            var exists = await _ctx.Boxes.AnyAsync(b => b.Id == box.Id);
            if (!exists)
                throw new DomainExceptionValidation.EntityNotFoundException($"Box {box.Id} não encontrado");

            _ctx.Boxes.Update(box);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _ctx.Boxes.FindAsync(id);
            if (entity is null)
                throw new DomainExceptionValidation.EntityNotFoundException($"Box {id} não encontrado");

            _ctx.Boxes.Remove(entity);
            await _ctx.SaveChangesAsync();
        }
    }
}

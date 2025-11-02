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

        public async Task<List<Box>> GetAllAsync(CancellationToken ct = default) =>
            await _ctx.Boxes.AsNoTracking().ToListAsync(ct);

        public async Task<Box?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _ctx.Boxes.FindAsync(new object[] { id }, ct);

        public async Task<Box> AddAsync(Box box, CancellationToken ct = default)
        {
            _ctx.Boxes.Add(box);
            await _ctx.SaveChangesAsync(ct);
            return box;
        }

        public async Task UpdateAsync(Box box, CancellationToken ct = default)
        {
            var exists = await _ctx.Boxes.AnyAsync(b => b.Id == box.Id, ct);
            if (!exists)
                throw new DomainExceptionValidation.EntityNotFoundException($"Box {box.Id} não encontrado");

            _ctx.Boxes.Update(box);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _ctx.Boxes.FindAsync(new object[] { id }, ct);
            if (entity is null)
                throw new DomainExceptionValidation.EntityNotFoundException($"Box {id} não encontrado");

            _ctx.Boxes.Remove(entity);
            await _ctx.SaveChangesAsync(ct);
        }

    }
}
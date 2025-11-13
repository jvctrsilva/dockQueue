using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using DockQueue.Domain.Validation;

namespace DockQueue.Infra.Data.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly ApplicationDbContext _ctx;
        public StatusRepository(ApplicationDbContext ctx) { _ctx = ctx; }

        public async Task<List<Status>> GetAllAsync(CancellationToken ct = default)
            => await _ctx.Statuses.OrderBy(s => s.DisplayOrder).ThenBy(s => s.Name).ToListAsync();

        public async Task<Status?> GetByCodeAsync(string code)
            => await _ctx.Statuses.FirstOrDefaultAsync(s => s.Code == code);

        public async Task<Status> GetByIdAsync(int id)
            => await _ctx.Statuses.FindAsync(id)
                ?? throw new DomainExceptionValidation.EntityNotFoundException($"Status id {id} não encontrado");

        public async Task<Status> AddAsync(Status status)
        {
            _ctx.Statuses.Add(status);
            await _ctx.SaveChangesAsync();
            return status;
        }

        public async Task UpdateAsync(Status status)
        {
            _ctx.Statuses.Update(status);
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> ExistsDefaultAsync(int? ignoreId = null)
        {
            return await _ctx.Statuses
                .AnyAsync(s => s.IsDefault && (!ignoreId.HasValue || s.Id != ignoreId.Value));
        }

        public async Task DeleteAsync(int id)
        {
            var status = await GetByIdAsync(id);
            _ctx.Statuses.Remove(status);
            await _ctx.SaveChangesAsync();
        }
        public async Task<Status?> GetFirstBySequenceAsync()
        {
            // Ajuste "Sequence" para o nome exato da sua coluna/propriedade (Sequencia, Order, etc.)
            return await _ctx.Statuses
                .OrderBy(s => s.DisplayOrder)
                .FirstOrDefaultAsync();
        }

        public async Task<Status?> GetDefaultStatusAsync()
        {
            return await _ctx.Statuses
                .FirstOrDefaultAsync(s => s.IsDefault);
        }

        public async Task<bool> ExistsFinalStatusAsync(int? ignoreId = null)
        {
            return await _ctx.Statuses
                .AnyAsync(s => s.IsDefault && s.IsTerminal && s.Active && (!ignoreId.HasValue || s.Id != ignoreId.Value));
        }
    }
}

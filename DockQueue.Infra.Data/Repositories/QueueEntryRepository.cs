using DockQueue.Domain.Entities;
using DockQueue.Domain.Enums;
using DockQueue.Domain.Interfaces;
using DockQueue.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DockQueue.Infra.Data.Repositories
{
    public class QueueEntryRepository : IQueueEntryRepository
    {
        private readonly ApplicationDbContext _ctx;

        public QueueEntryRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<QueueEntry> AddAsync(QueueEntry entry)
        {
            _ctx.QueueEntries.Add(entry);
            await _ctx.SaveChangesAsync();
            return entry;
        }

        public async Task<QueueEntry?> GetByIdAsync(int id)
        {
            return await _ctx.QueueEntries
                .Include(q => q.Driver)
                .Include(q => q.Status)
                .Include(q => q.Box)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IReadOnlyList<QueueEntry>> GetQueueAsync(QueueType type)
        {
            return await _ctx.QueueEntries
                .Include(q => q.Driver)
                .Include(q => q.Status)
                .Include(q => q.Box)
                .Where(q => q.Type == type && !q.Status.IsDefault) // Não mostra status padrão (finalizados)
                .OrderBy(q => q.Priority ?? int.MaxValue)
                .ThenBy(q => q.Position)
                .ToListAsync();
        }

        public async Task UpdateAsync(QueueEntry entry)
        {
            _ctx.QueueEntries.Update(entry);
            await _ctx.SaveChangesAsync();
        }

        public async Task AddHistoryAsync(QueueEntryStatusHistory history)
        {
            _ctx.QueueEntryStatusHistory.Add(history);
            await _ctx.SaveChangesAsync();
        }

        public async Task<int> GetNextPositionAsync(QueueType type)
        {
            var max = await _ctx.QueueEntries
                .Where(q => q.Type == type)
                .MaxAsync(q => (int?)q.Position) ?? 0;

            return max + 1;
        }
        public async Task<QueueEntry?> GetActiveByDriverAsync(QueueType type, string documentNumber, string vehiclePlate)
        {
            return await _ctx.QueueEntries
                .Include(q => q.Driver)
                .Include(q => q.Status)
                .Include(q => q.Box)
                .Where(q => q.Type == type
                       && q.Driver.DocumentNumber == documentNumber
                       && q.Driver.VehiclePlate == vehiclePlate)
                .OrderByDescending(q => q.CreatedAt) // se tiver mais de uma, pega a mais recente
                .FirstOrDefaultAsync();
        }
        public async Task ClearQueueAsync(QueueType type)
        {
            var entries = _ctx.QueueEntries
                .Where(q => q.Type == type);

            _ctx.QueueEntries.RemoveRange(entries);
            await _ctx.SaveChangesAsync();
        }
        public async Task<bool> HasActiveEntryByDocumentAsync(QueueType type, string documentNumber)
        {
            return await _ctx.QueueEntries
                .Include(q => q.Driver)
                .Include(q => q.Status)
                .AnyAsync(q =>
                    q.Type == type &&
                    q.Driver.DocumentNumber == documentNumber);
        }

        public async Task<bool> RemoveEntry(QueueEntry entry)
        {
            _ctx.QueueEntries.Remove(entry);
            var changes = await _ctx.SaveChangesAsync();
            return changes > 0;
        }
    }
}

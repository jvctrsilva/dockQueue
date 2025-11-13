using DockQueue.Domain.Entities;
using DockQueue.Domain.Enums;

namespace DockQueue.Domain.Interfaces
{
    public interface IQueueEntryRepository
    {
        Task<QueueEntry> AddAsync(QueueEntry entry);
        Task<QueueEntry?> GetByIdAsync(int id);
        Task<IReadOnlyList<QueueEntry>> GetQueueAsync(QueueType type);
        Task UpdateAsync(QueueEntry entry);
        Task AddHistoryAsync(QueueEntryStatusHistory history);
        Task<int> GetNextPositionAsync(QueueType type);
        Task<QueueEntry?> GetActiveByDriverAsync(QueueType type, string documentNumber, string vehiclePlate);
        Task ClearQueueAsync(QueueType type);
        Task<bool> HasActiveEntryByDocumentAsync(QueueType type, string documentNumber);
    }
}

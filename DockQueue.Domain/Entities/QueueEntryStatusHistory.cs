namespace DockQueue.Domain.Entities
{
    public class QueueEntryStatusHistory
    {
        public int Id { get; set; }

        public int QueueEntryId { get; set; }
        public QueueEntry QueueEntry { get; set; } = null!;

        public int OldStatusId { get; set; }
        public int NewStatusId { get; set; }

        public DateTime ChangedAt { get; set; }

        public int? ChangedByUserId { get; set; }
        public User? ChangedByUser { get; set; }

        public QueueEntryStatusHistory(
            int queueEntryId,
            int oldStatusId,
            int newStatusId,
            int? changedByUserId)
        {
            QueueEntryId = queueEntryId;
            OldStatusId = oldStatusId;
            NewStatusId = newStatusId;
            ChangedByUserId = changedByUserId;
            ChangedAt = DateTime.UtcNow;
        }

        public QueueEntryStatusHistory() { }
    }
}

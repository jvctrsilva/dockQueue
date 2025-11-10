using DockQueue.Domain.Enums;

namespace DockQueue.Application.DTOs
{
    public class CreateQueueEntryDto
    {
        public QueueType Type { get; set; } // Loading / Unloading

        // Dados do motorista
        public string DriverName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
    }

    public class UpdateQueueEntryStatusDto
    {
        public int QueueEntryId { get; set; }
        public int NewStatusId { get; set; }
    }

    public class AssignBoxDto
    {
        public int QueueEntryId { get; set; }
        public int BoxId { get; set; }
    }
    public class QueueEntryViewDto
    {
        public int Id { get; set; }
        public QueueType Type { get; set; }

        public int DriverId { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public int Position { get; set; }
        public int? Priority { get; set; }

        public int StatusId { get; set; }
        public string StatusName { get; set; } = string.Empty;

        public int? BoxId { get; set; }
        public string? BoxName { get; set; }
    }
}

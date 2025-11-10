using DockQueue.Domain.Enums;

namespace DockQueue.Application.DTOs
{
    public class DriverQueueLookupDto
    {
        public QueueType Type { get; set; } // Loading / Unloading
        public string DocumentNumber { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;
    }
}

using DockQueue.Domain.ValueObjects;

namespace DockQueue.Application.DTOs
{
    public class SystemSettingsDto
    {
        public OperatingDays OperatingDays { get; set; }
        public string StartTime { get; set; } = default!; // HH:mm
        public string EndTime { get; set; } = default!;
        public string TimeZone { get; set; } = "America/Sao_Paulo";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateSystemSettingsDto
    {
        public OperatingDays OperatingDays { get; set; }
        public string StartTime { get; set; } = default!; // HH:mm
        public string EndTime { get; set; } = default!;
        public string TimeZone { get; set; } = "America/Sao_Paulo";
    }
}

using System;

namespace DockQueue.Client.Shared
{
    [Flags]
    public enum OperatingDays
    {
        None = 0, Monday = 1, Tuesday = 2, Wednesday = 4, Thursday = 8, Friday = 16, Saturday = 32, Sunday = 64,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        All = Weekdays | Saturday | Sunday
    }

    // DockQueue.Client.Shared
    public class SettingsDto
    {
        public OperatingDays OperatingDays { get; set; }
        public string? StartTime { get; set; } // "HH:mm" ou null
        public string? EndTime { get; set; } // "HH:mm" ou null
        public string TimeZone { get; set; } = "America/Sao_Paulo";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class UpdateSettingsDto
    {
        public OperatingDays OperatingDays { get; set; }
        public string? StartTime { get; set; } // "HH:mm" ou null
        public string? EndTime { get; set; } // "HH:mm" ou null
        public string TimeZone { get; set; } = "America/Sao_Paulo";
    }

}

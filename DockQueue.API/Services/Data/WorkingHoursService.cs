namespace DockQueue.Services.UI;

public class WorkingHoursService
{
    private static WeeklyWorkingHours? _cache;

    public async Task<WeeklyWorkingHours> GetAsync()
    {
        // chamada api?
        await Task.Delay(100);
        return _cache ??= WeeklyWorkingHours.CreateDefault();
    }

    public async Task SaveAsync(WeeklyWorkingHours workingHours)
    {
        // chamada api?
        await Task.Delay(100);
        _cache = workingHours;
    }
}

public class WeeklyWorkingHours
{
    public List<DayWorkingHours> Days { get; set; } = new();

    public static WeeklyWorkingHours CreateDefault()
    {
        var weekly = new WeeklyWorkingHours();
        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            var isBusinessDay = day is not DayOfWeek.Saturday and not DayOfWeek.Sunday;
            weekly.Days.Add(new DayWorkingHours
            {
                Day = day,
                Open = isBusinessDay,
                StartTime = isBusinessDay ? new TimeSpan(8, 0, 0) : null,
                EndTime = isBusinessDay ? new TimeSpan(18, 0, 0) : null
            });
        }
        return weekly;
    }
}

public class DayWorkingHours
{
    public DayOfWeek Day { get; set; }
    public bool Open { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
}
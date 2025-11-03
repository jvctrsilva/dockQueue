using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Interfaces;
using DockQueue.Domain.Validation;
using System.Globalization;

public class SystemSettingsService : ISystemSettingsService
{
    private readonly ISystemSettingsRepository _repo;
    public SystemSettingsService(ISystemSettingsRepository repo) { _repo = repo; }

    public async Task<SystemSettingsDto?> GetAsync()
    {
        var s = await _repo.GetAsync();
        return s is null ? null : Map(s);
    }


    private static readonly string[] TIME_FORMATS = new[]
    {
    "HH:mm", "H:mm", "HH:mm:ss", "H:mm:ss"
    };

    public async Task<SystemSettingsDto> UpsertAsync(UpdateSystemSettingsDto dto)
    {
        var existing = await _repo.GetAsync();
        var nowUtc = DateTime.UtcNow;

        // START
        TimeOnly? start = existing?.StartTime;
        if (!string.IsNullOrWhiteSpace(dto.StartTime))
        {
            var s = dto.StartTime.Trim();
            if (!TimeOnly.TryParseExact(s, TIME_FORMATS, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out var st))
                throw new DomainExceptionValidation("StartTime inválido (use HH:mm)");
            start = st;
        }

        // END
        TimeOnly? end = existing?.EndTime;
        if (!string.IsNullOrWhiteSpace(dto.EndTime))
        {
            var s = dto.EndTime.Trim();
            if (!TimeOnly.TryParseExact(s, TIME_FORMATS, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out var et))
                throw new DomainExceptionValidation("EndTime inválido (use HH:mm)");
            end = et;
        }

        SystemSettings saved;
        if (existing is null)
        {
            var model = new SystemSettings(dto.OperatingDays, start, end, dto.TimeZone, nowUtc);
            saved = await _repo.UpsertAsync(model);
        }
        else
        {
            existing.Update(dto.OperatingDays, start, end, dto.TimeZone, nowUtc);
            saved = await _repo.UpsertAsync(existing);
        }

        return new SystemSettingsDto
        {
            OperatingDays = saved.OperatingDays,
            StartTime = saved.StartTime?.ToString("HH\\:mm"),
            EndTime = saved.EndTime?.ToString("HH\\:mm"),
            TimeZone = saved.TimeZone,
            CreatedAt = saved.CreatedAt,
            UpdatedAt = saved.UpdatedAt
        };
    }

    private static SystemSettingsDto Map(SystemSettings s) => new()
    {
        OperatingDays = s.OperatingDays,
        StartTime = s.StartTime.HasValue ? s.StartTime.Value.ToString("HH\\:mm") : null,
        EndTime = s.EndTime.HasValue ? s.EndTime.Value.ToString("HH\\:mm") : null,
        TimeZone = s.TimeZone,
        CreatedAt = s.CreatedAt,
        UpdatedAt = s.UpdatedAt
    };
}

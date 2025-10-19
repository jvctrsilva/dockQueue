using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Domain.Validation;

namespace DockQueue.Application.Services
{
	public class SystemSettingsService : ISystemSettingsService
	{
		private readonly ISystemSettingsRepository _repo;

		public SystemSettingsService(ISystemSettingsRepository repo) { _repo = repo; }

		public async Task<SystemSettingsDto?> GetAsync()
		{
			var s = await _repo.GetAsync();
			return s is null ? null : Map(s);
		}

		public async Task<SystemSettingsDto> UpsertAsync(UpdateSystemSettingsDto dto)
		{
			if (!TimeOnly.TryParse(dto.StartTime, out var start))
				throw new DomainExceptionValidation("StartTime inválido (use HH:mm)");

			if (!TimeOnly.TryParse(dto.EndTime, out var end))
				throw new DomainExceptionValidation("EndTime inválido (use HH:mm)");

			var nowUtc = DateTime.UtcNow;
			var incoming = new SystemSettings(dto.OperatingDays, start, end, dto.TimeZone, nowUtc);
			var saved = await _repo.UpsertAsync(incoming);
			return Map(saved);
		}

		private static SystemSettingsDto Map(SystemSettings s) => new()
		{
			OperatingDays = s.OperatingDays,
			StartTime = s.StartTime.ToString("HH\\:mm"),
			EndTime = s.EndTime.ToString("HH\\:mm"),
			TimeZone = s.TimeZone,
			CreatedAt = s.CreatedAt,
			UpdatedAt = s.UpdatedAt
		};
	}
}

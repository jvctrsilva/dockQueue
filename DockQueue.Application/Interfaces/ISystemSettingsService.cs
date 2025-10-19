using DockQueue.Application.DTOs;

namespace DockQueue.Application.Interfaces
{
    public interface ISystemSettingsService
    {
        Task<SystemSettingsDto?> GetAsync();
        Task<SystemSettingsDto> UpsertAsync(UpdateSystemSettingsDto dto);
    }
}

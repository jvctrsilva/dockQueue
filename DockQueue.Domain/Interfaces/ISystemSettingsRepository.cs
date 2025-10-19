using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface ISystemSettingsRepository
    {
        Task<SystemSettings?> GetAsync();
        Task<SystemSettings> UpsertAsync(SystemSettings settings);
    }
}

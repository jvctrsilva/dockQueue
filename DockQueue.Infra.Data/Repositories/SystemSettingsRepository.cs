using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DockQueue.Infra.Data.Repositories
{
    public class SystemSettingsRepository : ISystemSettingsRepository
    {
        private readonly ApplicationDbContext _ctx;
        public SystemSettingsRepository(ApplicationDbContext ctx) { _ctx = ctx; }

        public async Task<SystemSettings?> GetAsync()
            => await _ctx.SystemSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Id == 1);

        public async Task<SystemSettings> UpsertAsync(SystemSettings settings)
        {
            var existing = await _ctx.SystemSettings.FirstOrDefaultAsync(x => x.Id == 1);
            if (existing is null)
            {
                _ctx.SystemSettings.Add(settings);
            }
            else
            {
                existing.Update(settings.OperatingDays, settings.StartTime, settings.EndTime, settings.TimeZone, DateTime.UtcNow);
                _ctx.SystemSettings.Update(existing);
            }
            await _ctx.SaveChangesAsync();
            return await GetAsync() ?? throw new InvalidOperationException("Falha ao persistir SystemSettings");
        }
    }
}

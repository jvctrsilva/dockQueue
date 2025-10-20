using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DockQueue.Infra.Data.Repositories
{
    public class OperatorPermissionsRepository : IOperatorPermissionsRepository
    {
        private readonly ApplicationDbContext _ctx;
        public OperatorPermissionsRepository(ApplicationDbContext ctx) { _ctx = ctx; }

        // Carrega o agregado inteiro (telas + listas)
        public async Task<OperatorPermissions?> GetByUserIdAsync(int userId)
        {
            return await _ctx.OperatorPermissions
                .Include(x => x.AllowedStatuses)
                .Include(x => x.AllowedBoxes)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        // Upsert transacional simples
        public async Task<OperatorPermissions> UpsertAsync(OperatorPermissions incoming)
        {
            var existing = await _ctx.OperatorPermissions
                .Include(x => x.AllowedStatuses)
                .Include(x => x.AllowedBoxes)
                .FirstOrDefaultAsync(x => x.UserId == incoming.UserId);

            if (existing is null)
            {
                _ctx.OperatorPermissions.Add(incoming);
            }
            else
            {
                existing.UpdateScreens(incoming.AllowedScreens, DateTime.UtcNow);
                existing.SetStatuses(incoming.AllowedStatuses.Select(s => s.StatusId), DateTime.UtcNow);
                existing.SetBoxes(incoming.AllowedBoxes.Select(b => b.BoxId), DateTime.UtcNow);
                _ctx.OperatorPermissions.Update(existing);
            }

            await _ctx.SaveChangesAsync();
            return await GetByUserIdAsync(incoming.UserId) ?? throw new InvalidOperationException("Falha ao salvar permissões");
        }
    }
}

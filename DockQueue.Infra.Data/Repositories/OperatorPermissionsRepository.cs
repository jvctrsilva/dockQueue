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
        public async Task<OperatorPermissions?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _ctx.OperatorPermissions
                .AsNoTracking()
                .Include(x => x.AllowedStatuses)
                .Include(x => x.AllowedBoxes)
                .FirstOrDefaultAsync(x => x.UserId == userId, ct);
        }

        public async Task<OperatorPermissions> UpsertAsync(OperatorPermissions incoming, CancellationToken ct = default)
        {
            var existing = await _ctx.OperatorPermissions
                .Include(x => x.AllowedStatuses)
                .Include(x => x.AllowedBoxes)
                .FirstOrDefaultAsync(x => x.UserId == incoming.UserId, ct);

            var now = DateTime.UtcNow;

            if (existing is null)
            {
                incoming.UpdateScreens(incoming.AllowedScreens, now);

                // Blindagem: dedup ANTES de inserir
                var distinctStatusIds = incoming.AllowedStatuses.Select(s => s.StatusId).Where(id => id > 0).Distinct();
                incoming.SetStatuses(distinctStatusIds, now);

                var distinctBoxIds = incoming.AllowedBoxes.Select(b => b.BoxId).Where(id => id > 0).Distinct();
                incoming.SetBoxes(distinctBoxIds, now);

                _ctx.OperatorPermissions.Add(incoming);
                await _ctx.SaveChangesAsync(ct);

                return await _ctx.OperatorPermissions
                    .Include(x => x.AllowedStatuses)
                    .Include(x => x.AllowedBoxes)
                    .FirstAsync(x => x.UserId == incoming.UserId, ct);
            }

            // Atualiza telas
            existing.UpdateScreens(incoming.AllowedScreens, now);

            // ========================
            // STATUSES
            // ========================
            var desiredStatusIds = incoming.AllowedStatuses
                .Select(s => s.StatusId)
                .Where(id => id > 0)
                .Distinct()
                .ToHashSet();

            // remover os que não estão mais
            var toRemoveStatus = existing.AllowedStatuses
                .Where(s => !desiredStatusIds.Contains(s.StatusId))
                .ToList();

            if (toRemoveStatus.Count > 0)
            {
                _ctx.RemoveRange(toRemoveStatus);
                await _ctx.SaveChangesAsync(ct); // salva remoções antes de inserir
            }

            // adicionar faltantes, checando no banco
            var userId = existing.UserId;
            foreach (var statusId in desiredStatusIds)
            {
                bool alreadyExists = await _ctx.Set<OperatorStatusPermission>()
                    .AnyAsync(x => x.UserId == userId && x.StatusId == statusId, ct);

                if (!alreadyExists)
                {
                    existing.AllowedStatuses.Add(new OperatorStatusPermission(userId, statusId));
                }
            }

            // ========================
            // BOXES
            // ========================
            var desiredBoxIds = incoming.AllowedBoxes
                .Select(b => b.BoxId)
                .Where(id => id > 0)
                .Distinct()
                .ToHashSet();

            var toRemoveBoxes = existing.AllowedBoxes
                .Where(b => !desiredBoxIds.Contains(b.BoxId))
                .ToList();

            if (toRemoveBoxes.Count > 0)
            {
                _ctx.RemoveRange(toRemoveBoxes);
                await _ctx.SaveChangesAsync(ct);
            }

            foreach (var boxId in desiredBoxIds)
            {
                bool alreadyExists = await _ctx.Set<OperatorBoxPermission>()
                    .AnyAsync(x => x.UserId == userId && x.BoxId == boxId, ct);

                if (!alreadyExists)
                {
                    existing.AllowedBoxes.Add(new OperatorBoxPermission(userId, boxId));
                }
            }

            await _ctx.SaveChangesAsync(ct);

            return await _ctx.OperatorPermissions
                .Include(x => x.AllowedStatuses)
                .Include(x => x.AllowedBoxes)
                .FirstAsync(x => x.UserId == incoming.UserId, ct);
        }

    }
}

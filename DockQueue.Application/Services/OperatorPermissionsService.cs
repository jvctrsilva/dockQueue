using DockQueue.Application.DTOs.Permissions;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Domain.Validation;

namespace DockQueue.Application.Services
{
    public class OperatorPermissionsService : IOperatorPermissionsService
    {
        private readonly IOperatorPermissionsRepository _repo;
        private readonly IUserRepository _userRepo; // valida se user existe
        private readonly IStatusRepository _statusRepo; // (opcional) valida ids
        // Se quiser validar boxes, injete IBoxRepository (se existir)

        public OperatorPermissionsService(
            IOperatorPermissionsRepository repo,
            IUserRepository userRepo,
            IStatusRepository statusRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _statusRepo = statusRepo;
        }

        public async Task<OperatorPermissionsDto?> GetByUserIdAsync(int userId)
        {
            var agg = await _repo.GetByUserIdAsync(userId);
            return agg is null ? null : Map(agg);
        }

        public async Task<OperatorPermissionsDto> UpsertAsync(int userId, UpdateOperatorPermissionsDto dto)
        {
            // --- Valida usuário existente (feedback claro para tela) ---
            _ = await _userRepo.GetByIdAsync(userId);

            // (Opcional) Validar se StatusIds existem (ajuda a evitar “lixo”)
            // var allStatuses = await _statusRepo.GetAllAsync();
            // var set = allStatuses.Select(s => s.Id).ToHashSet();
            // if (dto.AllowedStatusIds.Any(id => !set.Contains(id)))
            //     throw new DomainExceptionValidation("Existem StatusIds inválidos");

            var now = DateTime.UtcNow;
            var incoming = new OperatorPermissions(userId, dto.AllowedScreens, now);
            incoming.SetStatuses(dto.AllowedStatusIds, now);
            incoming.SetBoxes(dto.AllowedBoxIds, now);

            var saved = await _repo.UpsertAsync(incoming);
            return Map(saved);
        }

        private static OperatorPermissionsDto Map(OperatorPermissions x) => new()
        {
            UserId = x.UserId,
            AllowedScreens = x.AllowedScreens,
            AllowedStatusIds = x.AllowedStatuses.Select(s => s.StatusId).ToList(),
            AllowedBoxIds = x.AllowedBoxes.Select(b => b.BoxId).ToList(),
            UpdatedAt = x.UpdatedAt
        };
    }
}

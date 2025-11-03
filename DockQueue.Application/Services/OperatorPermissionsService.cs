using DockQueue.Application.DTOs;
using DockQueue.Application.DTOs.Permissions;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Domain.Validation;
using DockQueue.Domain.ValueObjects;

namespace DockQueue.Application.Services
{
    public class OperatorPermissionsService : IOperatorPermissionsService
    {
        private readonly IOperatorPermissionsRepository _permRepo;
        private readonly IUserRepository _userRepo;
        private readonly IStatusRepository _statusRepo;
        private readonly IBoxRepository _boxRepo;

        public OperatorPermissionsService(
            IOperatorPermissionsRepository permRepo,
            IUserRepository userRepo,
            IStatusRepository statusRepo,
            IBoxRepository boxRepo)
        {
            _permRepo = permRepo;
            _userRepo = userRepo;
            _statusRepo = statusRepo;
            _boxRepo = boxRepo;
        }

        public async Task<OperatorPermissionsDto?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            var agg = await _permRepo.GetByUserIdAsync(userId, ct);
            return agg is null ? null : Map(agg);
        }

        public async Task<OperatorPermissionsDto> UpsertAsync(int userId, UpdateOperatorPermissionsDto dto, CancellationToken ct = default)
        {
            // Garante que o usuário existe (lança se não existir)
            _ = await _userRepo.GetByIdAsync(userId);

            var now = DateTime.UtcNow;

            var incoming = new OperatorPermissions(userId, dto.AllowedScreens, now);
            incoming.SetStatuses(dto.AllowedStatusIds, now); // popula AllowedStatuses (entities filho)
            incoming.SetBoxes(dto.AllowedBoxIds, now);       // popula AllowedBoxes

            var saved = await _permRepo.UpsertAsync(incoming, ct);
            return Map(saved);
        }

        // → usado pela tela para carregar tudo de uma vez
        public async Task<PermissionsScreenDataDto?> GetScreenDataAsync(int userId, CancellationToken ct = default)
        {
            var statuses = await _statusRepo.GetAllAsync(ct);
            var boxes = await _boxRepo.GetAllAsync(ct);
            var perms = await _permRepo.GetByUserIdAsync(userId, ct);

            var dto = new PermissionsScreenDataDto
            {
                AllStatuses = statuses.Select(s => new StatusDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    Description = s.Description,
                    DisplayOrder = s.DisplayOrder,
                    IsDefault = s.IsDefault,
                    IsTerminal = s.IsTerminal,
                    Active = s.Active,
                    CreatedAt = s.CreatedAt
                }).ToList(),

                AllBoxes = boxes.Select(b => new BoxDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Status = b.Status,     // bool
                    DriverId = b.DriverId,   // int?
                    CreatedAt = b.CreatedAt
                }).ToList(),

                UserPermissions = perms is null
                      ? new OperatorPermissionsDto
                      {
                          UserId = userId,
                          AllowedStatusIds = new(),
                          AllowedBoxIds = new(),
                          AllowedScreens = Screen.None,
                          UpdatedAt = DateTime.UtcNow
                      }
                      : new OperatorPermissionsDto
                      {
                          UserId = perms.UserId,
                          AllowedStatusIds = perms.AllowedStatuses.Select(s => s.StatusId).ToList(),
                          AllowedBoxIds = perms.AllowedBoxes.Select(b => b.BoxId).ToList(),
                          AllowedScreens = perms.AllowedScreens,
                          UpdatedAt = perms.UpdatedAt
                      }
            };

            return dto;
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

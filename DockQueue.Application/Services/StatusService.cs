using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Domain.Validation;

namespace DockQueue.Application.Services
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _repo;

        public StatusService(IStatusRepository repo) { _repo = repo; }

        public async Task<List<StatusDto>> GetAllAsync()
            => (await _repo.GetAllAsync()).Select(Map).ToList();

        public async Task<StatusDto> GetByIdAsync(int id) => Map(await _repo.GetByIdAsync(id));

        public async Task<StatusDto> CreateAsync(CreateStatusDto dto)
        {
            var existing = await _repo.GetByCodeAsync(dto.Code.Trim().ToUpperInvariant());
            if (existing is not null)
                throw new DomainExceptionValidation("Já existe status com esse código");

            if (dto.IsDefault && await _repo.ExistsDefaultAsync())
                throw new DomainExceptionValidation("Já existe um status final");

            // Validação: não permite mais de um status final ativo (IsDefault = true E IsTerminal = true)
            if (dto.IsDefault && dto.IsTerminal && dto.Active && await _repo.ExistsFinalStatusAsync())
                throw new DomainExceptionValidation("Já existe um status final ativo. Desative o status final existente antes de criar outro.");

            var entity = new Status(
                dto.Code, 
                dto.Name,
                dto.Description,
                dto.DisplayOrder, 
                dto.IsDefault, 
                dto.IsTerminal,
                dto.Active, DateTime.UtcNow
            );

            entity = await _repo.AddAsync(entity);
            return Map(entity);
        }

        public async Task<StatusDto> UpdateAsync(int id, UpdateStatusDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);

            var otherWithSameCode = await _repo.GetByCodeAsync(dto.Code.Trim().ToUpperInvariant());
            if (otherWithSameCode is not null && otherWithSameCode.Id != id)
                throw new DomainExceptionValidation("Já existe status com esse código");

            // Regra do IsDefault: impede ficar com 2 status finais
            var willBeDefault = dto.IsDefault;
            if (willBeDefault && await _repo.ExistsDefaultAsync(ignoreId: id))
                throw new DomainExceptionValidation("Já existe um status final");

            // Validação: não permite mais de um status final ativo (IsDefault = true E IsTerminal = true)
            if (dto.IsDefault && dto.IsTerminal && dto.Active && await _repo.ExistsFinalStatusAsync(ignoreId: id))
                throw new DomainExceptionValidation("Já existe um status final ativo. Desative o status final existente antes de ativar outro.");

            entity.Update(dto.Code, dto.Name, dto.Description,
                          dto.DisplayOrder, dto.IsDefault, dto.IsTerminal, dto.Active);

            await _repo.UpdateAsync(entity);
            return Map(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }

        private static StatusDto Map(Status s) => new()
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
        };
    }
}

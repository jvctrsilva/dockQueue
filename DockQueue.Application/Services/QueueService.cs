using DockQueue.Application.DTOs;
using DockQueue.Domain.Entities;
using DockQueue.Domain.Enums;
using DockQueue.Domain.Interfaces;

namespace DockQueue.Application.Services
{
    public interface IQueueService
    {
        Task<QueueEntryViewDto> EnqueueAsync(CreateQueueEntryDto dto, int? userId = null);
        Task<IReadOnlyList<QueueEntryViewDto>> GetQueueAsync(QueueType type);
        Task<QueueEntryViewDto> UpdateStatusAsync(UpdateQueueEntryStatusDto dto, int? userId = null);
        Task<QueueEntryViewDto> AssignBoxAsync(AssignBoxDto dto, int? userId = null);
        Task<QueueEntryViewDto?> GetDriverQueueEntryAsync(DriverQueueLookupDto dto);
        Task<QueueEntryViewDto> StartBoxOperationAsync(StartBoxOperationDto dto, int? userId = null);
        Task<QueueEntryViewDto> FinishBoxOperationAsync(FinishBoxOperationDto dto, int? userId = null);

        Task ClearQueueAsync(QueueType type);
    }

    public class QueueService : IQueueService
    {
        private readonly IQueueEntryRepository _queueRepo;
        private readonly IDriverRepository _driverRepo; // você cria essa interface se ainda não tiver
        private readonly IStatusRepository _statusRepo;
        private readonly IBoxRepository _boxRepo;

        public QueueService(
            IQueueEntryRepository queueRepo,
            IDriverRepository driverRepo,
            IStatusRepository statusRepo,
            IBoxRepository boxRepo)
        {
            _queueRepo = queueRepo;
            _driverRepo = driverRepo;
            _statusRepo = statusRepo;
            _boxRepo = boxRepo;
        }

        public async Task<QueueEntryViewDto> EnqueueAsync(CreateQueueEntryDto dto, int? userId = null)
        {
            // 0) Normaliza dados (garante tudo maiúsculo também no backend)
            dto.DriverName = dto.DriverName.Trim().ToUpperInvariant();
            dto.VehiclePlate = dto.VehiclePlate.Trim().ToUpperInvariant();
            dto.DocumentNumber = dto.DocumentNumber.Trim();

            // 1) Regra: não permitir duas entradas ativas com mesmo documento na mesma fila
            var alreadyInQueue = await _queueRepo.HasActiveEntryByDocumentAsync(dto.Type, dto.DocumentNumber);
            if (alreadyInQueue)
            {
                throw new InvalidOperationException("Já existe um motorista com esse documento na fila.");
            }

            // 2) Descobrir o status inicial:
            //    - se dto.InitialStatusId vier 0 ou não vier, usamos o primeiro status pela sequência

            int initialStatusId;
            var firstStatus = await _statusRepo.GetFirstBySequenceAsync()
                ?? throw new InvalidOperationException("Nenhum status configurado.");

            initialStatusId = firstStatus.Id;

 
            var driverQueue = await _queueRepo.GetActiveByDriverAsync(
                dto.Type,
                dto.DocumentNumber,
                dto.VehiclePlate
            );

            if (driverQueue != null)
            {
                // não permite atualizar placa ou nome
                throw new InvalidOperationException("Já existe um motorista com esse documento cadastrado.");
            }
            // 3) Criar Driver

            var driver = await _driverRepo.FindByDocumentAsync(dto.DocumentNumber, dto.VehiclePlate);

            if (driver == null)
            {
                driver = new Driver
                {
                    Name = dto.DriverName,
                    DocumentNumber = dto.DocumentNumber,
                    VehiclePlate = dto.VehiclePlate
                };

                await _driverRepo.AddAsync(driver);
            }
            else
            {
                driver.Name = dto.DriverName;
                driver.VehiclePlate = dto.VehiclePlate;
                await _driverRepo.UpdateAsync(driver);
            }

            // 4) Descobrir próxima posição na fila daquele tipo
            var position = await _queueRepo.GetNextPositionAsync(dto.Type);

            // 5) Criar a entrada na fila com o status inicial calculado
            var entry = new QueueEntry(
                dto.Type,
                driver.Id,
                initialStatusId,
                position
            );

            await _queueRepo.AddAsync(entry);

            return MapToView(entry, driver);
        }

        public async Task<IReadOnlyList<QueueEntryViewDto>> GetQueueAsync(QueueType type)
        {
            var entries = await _queueRepo.GetQueueAsync(type);

            return entries.Select(e => MapToView(e, e.Driver)).ToList();
        }

        public async Task<QueueEntryViewDto> UpdateStatusAsync(UpdateQueueEntryStatusDto dto, int? userId = null)
        {
            var entry = await _queueRepo.GetByIdAsync(dto.QueueEntryId)
                        ?? throw new Exception("Queue entry not found");

            var oldStatusId = entry.StatusId;
            entry.UpdateStatus(dto.NewStatusId, userId);

            await _queueRepo.UpdateAsync(entry);

            var history = new QueueEntryStatusHistory(
                entry.Id,
                oldStatusId,
                dto.NewStatusId,
                userId
            );

            await _queueRepo.AddHistoryAsync(history);

            return MapToView(entry, entry.Driver);
        }

        public async Task<QueueEntryViewDto> AssignBoxAsync(AssignBoxDto dto, int? userId = null)
        {
            var entry = await _queueRepo.GetByIdAsync(dto.QueueEntryId)
                        ?? throw new Exception("Queue entry not found");

            if (dto.BoxId.HasValue)
            {
                // Verifica se o box já está ocupado por outro motorista
                var box = await _boxRepo.GetByIdAsync(dto.BoxId.Value);
                if (box == null)
                {
                    throw new Exception("Box não encontrado.");
                }

                if (box.DriverId.HasValue && box.DriverId.Value != entry.DriverId)
                {
                    throw new InvalidOperationException("Este box já está ocupado por outro motorista.");
                }

                entry.AssignBox(dto.BoxId.Value, userId);
            }
            else
            {
                entry.UnassignBox(userId);
            }
            
            await _queueRepo.UpdateAsync(entry);

            return MapToView(entry, entry.Driver);
        }

        private static QueueEntryViewDto MapToView(QueueEntry e, Driver d)
        {
            return new QueueEntryViewDto
            {
                Id = e.Id,
                Type = e.Type,
                DriverId = d.Id,
                DriverName = d.Name,
                DocumentNumber = d.DocumentNumber,
                VehiclePlate = d.VehiclePlate,
                CreatedAt = e.CreatedAt,
                Position = e.Position,
                Priority = e.Priority,
                StatusId = e.StatusId,
                StatusName = e.Status?.Name ?? string.Empty,
                BoxId = e.BoxId,
                BoxName = e.Box?.Name,
                BoxInOperation = e.Box?.DriverId != null && e.Box.DriverId == d.Id
            };
        }

        public async Task<QueueEntryViewDto?> GetDriverQueueEntryAsync(DriverQueueLookupDto dto)
        {
            var entry = await _queueRepo.GetActiveByDriverAsync(
                dto.Type,
                dto.DocumentNumber,
                dto.VehiclePlate
            );

            if (entry == null)
                return null;

            return MapToView(entry, entry.Driver);
        }
        public async Task ClearQueueAsync(QueueType type)
        {
            await _queueRepo.ClearQueueAsync(type);
        }

        public async Task<QueueEntryViewDto> StartBoxOperationAsync(StartBoxOperationDto dto, int? userId = null)
        {
            var entry = await _queueRepo.GetByIdAsync(dto.QueueEntryId)
                        ?? throw new Exception("Queue entry not found");

            if (!entry.BoxId.HasValue)
            {
                throw new InvalidOperationException("Motorista não possui box atribuído.");
            }

            var box = await _boxRepo.GetByIdAsync(entry.BoxId.Value);
            if (box == null)
            {
                throw new Exception("Box não encontrado.");
            }

            // Atualiza Box.DriverId para indicar que está em operação
            box.Update(box.Name, true, entry.DriverId, box.CreatedAt);
            await _boxRepo.UpdateAsync(box);

            // Recarrega a entrada com o box atualizado
            entry = await _queueRepo.GetByIdAsync(dto.QueueEntryId)
                    ?? throw new Exception("Queue entry not found");

            // Atualiza LastUpdatedByUserId na entrada
            entry.UpdateStatus(entry.StatusId, userId);
            await _queueRepo.UpdateAsync(entry);

            // Recarrega novamente para ter o box atualizado
            entry = await _queueRepo.GetByIdAsync(dto.QueueEntryId)
                    ?? throw new Exception("Queue entry not found");

            return MapToView(entry, entry.Driver);
        }

        public async Task<QueueEntryViewDto> FinishBoxOperationAsync(FinishBoxOperationDto dto, int? userId = null)
        {
            var entry = await _queueRepo.GetByIdAsync(dto.QueueEntryId)
                        ?? throw new Exception("Queue entry not found");

            if (!entry.BoxId.HasValue)
            {
                throw new InvalidOperationException("Motorista não possui box atribuído.");
            }

            var box = await _boxRepo.GetByIdAsync(entry.BoxId.Value);
            if (box == null)
            {
                throw new Exception("Box não encontrado.");
            }

            // Busca o status padrão (finalizado)
            var defaultStatus = await _statusRepo.GetDefaultStatusAsync()
                ?? throw new InvalidOperationException("Status padrão não configurado.");

            // Remove DriverId do box (libera o box)
            box.Update(box.Name, false, null, box.CreatedAt);
            await _boxRepo.UpdateAsync(box);

            // Atualiza status para o padrão (finalizado)
            var oldStatusId = entry.StatusId;
            entry.UpdateStatus(defaultStatus.Id, userId);
            await _queueRepo.UpdateAsync(entry);

            // Registra histórico
            var history = new QueueEntryStatusHistory(
                entry.Id,
                oldStatusId,
                defaultStatus.Id,
                userId
            );
            await _queueRepo.AddHistoryAsync(history);

            await _queueRepo.RemoveEntry(entry);

            // Recarrega a entrada para ter os dados atualizados
            entry = await _queueRepo.GetByIdAsync(dto.QueueEntryId)
                    ?? throw new Exception("Queue entry not found");

            return MapToView(entry, entry.Driver);
        }
    }
}

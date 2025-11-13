using DockQueue.Application.DTOs;
using DockQueue.Client.Services;
using DockQueue.Domain.Enums;

namespace DockQueue.Client.ViewModels
{
    public class QueueViewModel
    {
        private readonly QueueService _queueService;
        private readonly BoxService _boxService;
        private readonly StatusesService _statusService; // supondo que você já tenha

        public bool IsLoading { get; private set; }
        public string? Error { get; private set; }

        public QueueType ActiveType { get; private set; } = QueueType.Unloading;

        public List<QueueEntryViewDto> Entries { get; private set; } = new();
        public List<BoxDto> Boxes { get; private set; } = new();
        public List<StatusDto> Statuses { get; private set; } = new();

        public QueueViewModel(
            QueueService queueService,
            BoxService boxService,
            StatusesService statusService)
        {
            _queueService = queueService;
            _boxService = boxService;
            _statusService = statusService;
        }

        public async Task InitAsync()
        {
            IsLoading = true;
            Error = null;

            try
            {
                // Carrega status e boxes uma vez
                Boxes = await _boxService.GetAllAsync();
                Statuses = await _statusService.GetAllAsync();

                await LoadQueueAsync();
            }
            catch (Exception ex)
            {
                Error = $"Erro ao carregar dados: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadQueueAsync()
        {
            IsLoading = true;
            Error = null;

            try
            {
                Entries = await _queueService.GetQueueAsync(ActiveType);
            }
            catch (Exception ex)
            {
                Error = $"Erro ao carregar fila: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task ChangeTypeAsync(QueueType type)
        {
            if (ActiveType == type) return;

            ActiveType = type;
            await LoadQueueAsync();
        }

        public async Task UpdateStatusAsync(int queueEntryId, int newStatusId)
        {
            Error = null;

            try
            {
                var dto = new UpdateQueueEntryStatusDto
                {
                    QueueEntryId = queueEntryId,
                    NewStatusId = newStatusId
                };

                var updated = await _queueService.UpdateStatusAsync(dto);
                if (updated == null)
                {
                    Error = "Não foi possível atualizar o status.";
                    return;
                }

                // Atualiza entry na lista
                var index = Entries.FindIndex(e => e.Id == queueEntryId);
                if (index >= 0)
                {
                    Entries[index] = updated;
                }
            }
            catch (HttpRequestException ex)
            {
                var message = ex.Message;
                var colonIndex = message.IndexOf(':');
                if (colonIndex > 0 && colonIndex < message.Length - 1)
                {
                    Error = message.Substring(colonIndex + 1).Trim();
                }
                else
                {
                    Error = message;
                }
            }
            catch (Exception ex)
            {
                Error = $"Erro ao atualizar status: {ex.Message}";
            }
        }

        public async Task AssignBoxAsync(int queueEntryId, int? boxId)
        {
            Error = null;

            try
            {
                var dto = new AssignBoxDto
                {
                    QueueEntryId = queueEntryId,
                    BoxId = boxId
                };

                var updated = await _queueService.AssignBoxAsync(dto);
                if (updated == null)
                {
                    Error = "Não foi possível atualizar o box.";
                    return;
                }

                Boxes = await _boxService.GetAllAsync();

                var index = Entries.FindIndex(e => e.Id == queueEntryId);
                if (index >= 0)
                {
                    Entries[index] = updated;
                }
            }
            catch (Exception ex)
            {
                Error = $"Erro ao atualizar box: {ex.Message}";
            }
        }

        public async Task ClearCurrentQueueAsync()
        {
            Error = null;

            try
            {
                var ok = await _queueService.ClearQueueAsync(ActiveType);
                if (!ok)
                {
                    Error = "Não foi possível limpar a fila.";
                    return;
                }

                // Depois de limpar, recarrega a fila (vai voltar vazia)
                Entries.Clear();
            }
            catch (Exception ex)
            {
                Error = $"Erro ao limpar fila: {ex.Message}";
            }
        }

        public async Task StartBoxOperationAsync(int queueEntryId)
        {
            Error = null;

            try
            {
                var dto = new StartBoxOperationDto
                {
                    QueueEntryId = queueEntryId
                };

                var updated = await _queueService.StartBoxOperationAsync(dto);
                if (updated == null)
                {
                    Error = "Não foi possível iniciar a operação no box.";
                    return;
                }

                Boxes = await _boxService.GetAllAsync();

                var index = Entries.FindIndex(e => e.Id == queueEntryId);
                if (index >= 0)
                {
                    Entries[index] = updated;
                }
            }
            catch (Exception ex)
            {
                Error = $"Erro ao iniciar operação: {ex.Message}";
            }
        }

        public async Task FinishBoxOperationAsync(int queueEntryId)
        {
            Error = null;

            try
            {
                var dto = new FinishBoxOperationDto
                {
                    QueueEntryId = queueEntryId
                };

                var updated = await _queueService.FinishBoxOperationAsync(dto);
                if (updated == null)
                {
                    Error = "Não foi possível finalizar a operação no box.";
                    return;
                }

                // Remove da lista pois o status foi atualizado para status final (finalizado)
                // e não será mais exibido na fila
                var index = Entries.FindIndex(e => e.Id == queueEntryId);
                if (index >= 0)
                {
                    Entries.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                Error = $"Erro ao finalizar operação: {ex.Message}";
            }
        }
    }
}

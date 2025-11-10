using DockQueue.Application.DTOs;
using DockQueue.Client.Services;
using DockQueue.Domain.Enums;

namespace DockQueue.Client.ViewModels
{
    public class DriverQueueViewModel
    {
        private readonly QueueService _queueService;
        private readonly SystemSettingsApi _systemSettingsApi;

        public bool IsLoading { get; private set; }
        public string? Error { get; private set; }
        public string? Info { get; private set; }

        // Formulário (front vai bindar direto nesses caras)
        public QueueType Type { get; set; } = QueueType.Unloading;
        public string DriverName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string VehiclePlate { get; set; } = string.Empty;

        // Resultado da última consulta/entrada
        public QueueEntryViewDto? CurrentEntry { get; private set; }

        public DriverQueueViewModel(QueueService queueService, SystemSettingsApi systemSettingsApi)
        {
            _queueService = queueService;
            _systemSettingsApi = systemSettingsApi;
        }

        public async Task<bool> EnqueueAsync()
        {
            ResetMessages();
            IsLoading = true;

            try
            {
                // validações básicas
                if (string.IsNullOrWhiteSpace(DriverName) ||
                    string.IsNullOrWhiteSpace(DocumentNumber) ||
                    string.IsNullOrWhiteSpace(VehiclePlate))
                {
                    Error = "Preencha nome, documento e placa.";
                    return false;
                }

                // normalizar
                DriverName = DriverName.Trim().ToUpperInvariant();
                VehiclePlate = VehiclePlate.Trim().ToUpperInvariant();
                DocumentNumber = DocumentNumber.Trim();

                // horário de funcionamento
                var isOpen = await _systemSettingsApi.IsOpenNowAsync();
                if (!isOpen)
                {
                    Error = "No momento o sistema está fora do horário de funcionamento.";
                    return false;
                }

                var dto = new CreateQueueEntryDto
                {
                    Type = Type,
                    DriverName = DriverName,
                    DocumentNumber = DocumentNumber,
                    VehiclePlate = VehiclePlate,
                };

                var result = await _queueService.EnqueueAsync(dto);
                if (result != null)
                {
                    CurrentEntry = result;
                    Info = "Você entrou na fila. Abaixo estão seus dados.";
                    return true;
                }

                Error = "Não foi possível entrar na fila.";
                return false;
            }
            catch (Exception ex)
            {
                // se você ajustar o backend pra mandar mensagem específica, pode tratar aqui
                if (ex.Message.Contains("Já existe um motorista com esse documento"))
                {
                    Error = "Você já está na fila com esse documento.";
                }
                else
                {
                    Error = $"Erro ao entrar na fila: {ex.Message}";
                }

                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }


        public async Task<bool> CheckPositionAsync()
        {
            ResetMessages();
            IsLoading = true;

            try
            {
                if (string.IsNullOrWhiteSpace(DocumentNumber) ||
                    string.IsNullOrWhiteSpace(VehiclePlate))
                {
                    Error = "Informe documento e placa para consultar.";
                    return false;
                }

                var dto = new DriverQueueLookupDto
                {
                    Type = Type,
                    DocumentNumber = DocumentNumber.Trim(),
                    VehiclePlate = VehiclePlate.Trim().ToUpperInvariant()
                };

                var result = await _queueService.LookupAsync(dto);
                if (result == null)
                {
                    CurrentEntry = null;
                    Error = "Nenhuma entrada ativa encontrada para esses dados.";
                    return false;
                }

                CurrentEntry = result;
                Info = "Posição atual encontrada.";
                return true;
            }
            catch (Exception ex)
            {
                Error = $"Erro ao consultar fila: {ex.Message}";
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ResetMessages()
        {
            Error = null;
            Info = null;
        }
    }
}

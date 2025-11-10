using DockQueue.Client.Shared;
using DockQueue.Client.ViewModels;
using System.Net.Http.Headers;

namespace DockQueue.Client.Services
{
    public class SystemSettingsApi
    {
        private readonly HttpClient _httpClient;
        private readonly AuthViewModel _auth;
        public SystemSettingsApi(IHttpClientFactory httpClientFactory, AuthViewModel auth)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _auth = auth;
        }

        private void AttachAuthHeader()
        {
            var token = _auth.AccessToken;
            if (!string.IsNullOrWhiteSpace(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            else
                _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        private sealed class WireDto
        {
            public OperatingDays OperatingDays { get; set; }
            public string? StartTime { get; set; }  // "HH:mm" ou null
            public string? EndTime { get; set; }  // "HH:mm" ou null
            public string? TimeZone { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public async Task<SettingsDto?> GetAsync()
        {
            AttachAuthHeader();
            var resp = await _httpClient.GetAsync("api/settings/operating-schedule");
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();

            var w = await resp.Content.ReadFromJsonAsync<WireDto>();
            if (w is null) return null;

            return new SettingsDto
            {
                OperatingDays = w.OperatingDays,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
                TimeZone = w.TimeZone ?? "America/Sao_Paulo",
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            };
        }

        public async Task<SettingsDto> UpsertAsync(UpdateSettingsDto dto)
        {
            AttachAuthHeader();
            var payload = new
            {
                dto.OperatingDays,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                dto.TimeZone
            };

            var resp = await _httpClient.PutAsJsonAsync("api/settings/operating-schedule", payload);
            resp.EnsureSuccessStatusCode();

            var w = await resp.Content.ReadFromJsonAsync<WireDto>();

            return new SettingsDto
            {
                OperatingDays = w!.OperatingDays,
                StartTime = w.StartTime,
                EndTime = w.EndTime,
                TimeZone = w.TimeZone ?? "America/Sao_Paulo",
                CreatedAt = w.CreatedAt,
                UpdatedAt = w.UpdatedAt
            };
        }


        /// Verifica se o horário atual está dentro do horário de funcionamento
        /// configurado no banco (dias + faixa de horário).
        public async Task<bool> IsOpenNowAsync(DateTime? now = null)
        {
            AttachAuthHeader();
            var settings = await GetAsync();
            if (settings is null)
            {
                // Se não houver configuração, você decide:
                // true = não bloquear, false = sempre bloquear
                return true;
            }

            var referenceTime = now ?? DateTime.Now;

            // 1) Verifica se hoje é um dia operacional
            var today = referenceTime.DayOfWeek;
            var todayFlag = today switch
            {
                DayOfWeek.Sunday => OperatingDays.Sunday,
                DayOfWeek.Monday => OperatingDays.Monday,
                DayOfWeek.Tuesday => OperatingDays.Tuesday,
                DayOfWeek.Wednesday => OperatingDays.Wednesday,
                DayOfWeek.Thursday => OperatingDays.Thursday,
                DayOfWeek.Friday => OperatingDays.Friday,
                DayOfWeek.Saturday => OperatingDays.Saturday,
                _ => OperatingDays.None
            };

            var openToday = (settings.OperatingDays & todayFlag) != 0;
            if (!openToday)
                return false;

            // 2) Verifica faixa de horário (StartTime / EndTime no formato "HH:mm")
            if (string.IsNullOrWhiteSpace(settings.StartTime) ||
                string.IsNullOrWhiteSpace(settings.EndTime))
            {
                // Se não tiver horário definido, você pode decidir:
                // true = considera aberto o dia todo, false = considera fechado
                return false;
            }

            if (!TimeSpan.TryParse(settings.StartTime, out var start) ||
                !TimeSpan.TryParse(settings.EndTime, out var end))
            {
                // Se as configs estiverem zoadas, por segurança bloqueia
                return false;
            }

            var time = referenceTime.TimeOfDay;

            // Simples: não estamos tratando janela que passa da meia-noite.
            return time >= start && time <= end;
        }

    }
}

using DockQueue.Client.Shared;
using DockQueue.Client.ViewModels; // (pode não precisar mais, depende de SettingsDto)
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DockQueue.Client.Services
{
    public class SystemSettingsApi
    {
        private readonly HttpClient _httpClient;
        private readonly AuthViewModel _auth;
        private readonly JwtAuthenticationStateProvider _authProvider;

        public SystemSettingsApi(IHttpClientFactory httpClientFactory,
            AuthViewModel auth, 
            JwtAuthenticationStateProvider authProvider)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _auth = auth;
            _authProvider = authProvider;
        }

        private sealed class WireDto
        {
            public OperatingDays OperatingDays { get; set; }
            public string? StartTime { get; set; }
            public string? EndTime { get; set; }
            public string? TimeZone { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }
        private void AttachAuthHeader()
        {
            var token = _auth.AccessToken;
            Console.WriteLine($"[StatusesService] AttachAuthHeader - token length = {token?.Length ?? 0}");

            if (!string.IsNullOrWhiteSpace(token))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            else
                _httpClient.DefaultRequestHeaders.Authorization = null;
        }
        private async Task<HttpResponseMessage> SendAsync(
                        Func<HttpClient, Task<HttpResponseMessage>> httpCall)
        {
            AttachAuthHeader();
            var response = await httpCall(_httpClient);

            Console.WriteLine($"[BoxService] Status = {(int)response.StatusCode}");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                Console.WriteLine("[BoxService] 401 -> limpando auth");
                // sessão morreu: limpa auth + deixa o Blazor tratar como não autenticado
                await _authProvider.ClearAuthDataAsync();
                throw new UnauthorizedAccessException("Sessão expirada ou inválida.");
            }

            return response;
        }

        public async Task<SettingsDto?> GetAsync()
        {
            AttachAuthHeader();
            var resp = await SendAsync(c => c.GetAsync("/api/settings/operating-schedule"));
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

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

            var resp = await SendAsync(c => c.PutAsJsonAsync("/api/settings/operating-schedule", payload));
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

        public async Task<bool> IsOpenNowAsync(DateTime? now = null)
        {
            AttachAuthHeader();
            var settings = await GetAsync();
            if (settings is null)
            {
                // tua regra: se não houver config, true ou false?
                return true;
            }

            var referenceTime = now ?? DateTime.Now;

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

            if (string.IsNullOrWhiteSpace(settings.StartTime) ||
                string.IsNullOrWhiteSpace(settings.EndTime))
            {
                return false;
            }

            if (!TimeSpan.TryParse(settings.StartTime, out var start) ||
                !TimeSpan.TryParse(settings.EndTime, out var end))
            {
                return false;
            }

            var time = referenceTime.TimeOfDay;
            return time >= start && time <= end;
        }
    }
}

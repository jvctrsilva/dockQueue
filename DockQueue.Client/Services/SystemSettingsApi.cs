using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DockQueue.Client.Shared;

namespace DockQueue.Client.Services
{
    public class SystemSettingsApi
    {
        private readonly HttpClient _http;
        public SystemSettingsApi(IHttpClientFactory f) => _http = f.CreateClient("ApiClient");

        // DTO "wire" que bate com a API (strings)
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
            var resp = await _http.GetAsync("api/settings/operating-schedule");
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
            var payload = new
            {
                dto.OperatingDays,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                dto.TimeZone
            };

            var resp = await _http.PutAsJsonAsync("api/settings/operating-schedule", payload);
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

    }
}

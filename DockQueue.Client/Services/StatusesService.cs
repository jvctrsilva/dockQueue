using System.Net.Http.Json;
using DockQueue.Application.DTOs;

namespace DockQueue.Client.Services
{
    public class StatusesService
    {
        private readonly HttpClient _http;
        public StatusesService(IHttpClientFactory f) => _http = f.CreateClient("ApiClient");

        public async Task<List<StatusDto>> GetAllAsync(CancellationToken ct = default)
            => await _http.GetFromJsonAsync<List<StatusDto>>("api/statuses", ct) ?? new();

        public async Task<StatusDto?> GetByIdAsync(int id, CancellationToken ct = default)
            => await _http.GetFromJsonAsync<StatusDto>($"api/statuses/{id}", ct);

        public async Task<StatusDto> CreateAsync(CreateStatusDto dto, CancellationToken ct = default)
        {
            var resp = await _http.PostAsJsonAsync("api/statuses", dto, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao criar status ({(int)resp.StatusCode}): {body}");
            return (await resp.Content.ReadFromJsonAsync<StatusDto>(cancellationToken: ct))!;
        }

        public async Task<StatusDto> UpdateAsync(int id, UpdateStatusDto dto, CancellationToken ct = default)
        {
            var resp = await _http.PutAsJsonAsync($"api/statuses/{id}", dto, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao atualizar status ({(int)resp.StatusCode}): {body}");
            return (await resp.Content.ReadFromJsonAsync<StatusDto>(cancellationToken: ct))!;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var resp = await _http.DeleteAsync($"api/statuses/{id}", ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao excluir status ({(int)resp.StatusCode}): {body}");
        }
    }
}

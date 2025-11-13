using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DockQueue.Client.Services
{
    public class StatusesService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthViewModel _auth;
        public StatusesService(IHttpClientFactory httpClientFactory, AuthViewModel auth)
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
        public async Task<List<StatusDto>> GetAllAsync(CancellationToken ct = default)
        {
            AttachAuthHeader();
            return await _httpClient.GetFromJsonAsync<List<StatusDto>>("api/statuses", ct) ?? new();
        }

        public async Task<StatusDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            AttachAuthHeader();
            return await _httpClient.GetFromJsonAsync<StatusDto>($"api/statuses/{id}", ct);
        }
        public async Task<StatusDto> CreateAsync(CreateStatusDto dto, CancellationToken ct = default)
        {
            AttachAuthHeader();
            var resp = await _httpClient.PostAsJsonAsync("api/statuses", dto, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao criar status ({(int)resp.StatusCode}): {body}");
            return (await resp.Content.ReadFromJsonAsync<StatusDto>(cancellationToken: ct))!;
        }

        public async Task<StatusDto> UpdateAsync(int id, UpdateStatusDto dto, CancellationToken ct = default)
        {
            AttachAuthHeader();
            var resp = await _httpClient.PutAsJsonAsync($"api/statuses/{id}", dto, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao atualizar status ({(int)resp.StatusCode}): {body}");
            return (await resp.Content.ReadFromJsonAsync<StatusDto>(cancellationToken: ct))!;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            AttachAuthHeader();
            var resp = await _httpClient.DeleteAsync($"api/statuses/{id}", ct);
            var body = await resp.Content.ReadAsStringAsync(ct);
            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao excluir status ({(int)resp.StatusCode}): {body}");
        }
    }
}

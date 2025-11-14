using Azure;
using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;


namespace DockQueue.Client.Services
{
    public class StatusesService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthViewModel _auth;
        private readonly JwtAuthenticationStateProvider _authProvider;

        public StatusesService(
            IHttpClientFactory httpClientFactory,
            AuthViewModel auth, JwtAuthenticationStateProvider authProvider)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _auth = auth;
            _authProvider = authProvider;
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
        public async Task<List<StatusDto>> GetAllAsync()
        {
            AttachAuthHeader();

            var response = await SendAsync(c => c.GetAsync("api/statuses"));
            response.EnsureSuccessStatusCode();

            var status = await response.Content.ReadFromJsonAsync<List<StatusDto>>()
            return status ?? new List<StatusDto>();             
        }

        public async Task<StatusDto?> GetByIdAsync(int id)
        {
            AttachAuthHeader();

            var response = await SendAsync(c => c.GetAsync($"api/statuses/{id}"));
            Console.WriteLine($"[StatusesService] Status GET /api/statuses/{id} = {(int)resp.StatusCode}");

            if (!response.IsSuccessStatusCode)
                return null;

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Sem permissão para consultar status.");

            return await response.Content.ReadFromJsonAsync<StatusDto>();
        }

        public async Task<StatusDto> CreateAsync(CreateStatusDto dto)
        {
            AttachAuthHeader();

            var resp = await SendAsync(c => c.PostAsJsonAsync("api/statuses", dto));
            var body = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"[StatusesService] Status POST /api/statuses = {(int)resp.StatusCode}");

            if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Sem permissão para criar status.");

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao criar status ({(int)resp.StatusCode}): {body}");

            return (await resp.Content.ReadFromJsonAsync<StatusDto>())!;
        }

        public async Task<StatusDto> UpdateAsync(int id, UpdateStatusDto dto)
        {
            AttachAuthHeader();

            var resp = await SendAsync(c => c.PutAsJsonAsync($"api/statuses/{id}", dto));
            var body = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"[StatusesService] Status PUT /api/statuses/{id} = {(int)resp.StatusCode}");

            if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Sem permissão para atualizar status.");

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao atualizar status ({(int)resp.StatusCode}): {body}");

            return (await resp.Content.ReadFromJsonAsync<StatusDto>())!;
        }

        public async Task DeleteAsync(int id)
        {
            AttachAuthHeader();

            var resp = await SendAsync(c => c.DeleteAsync($"api/statuses/{id}"));
            var body = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"[StatusesService] Status DELETE /api/statuses/{id} = {(int)resp.StatusCode}");

            if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Sem permissão para excluir status.");

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao excluir status ({(int)resp.StatusCode}): {body}");
        }
    }
}

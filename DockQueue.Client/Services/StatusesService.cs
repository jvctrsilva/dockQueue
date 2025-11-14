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
        public async Task<List<StatusDto>> GetAllAsync(CancellationToken ct = default)
        {
            AttachAuthHeader();

            var resp = await SendAsync(c => c.GetAsync("api/statuses", ct));
            Console.WriteLine($"[StatusesService] Status /api/statuses = {(int)resp.StatusCode}");

            if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                // aqui NÃO vamos deslogar o usuário
                // deixamos pra tela decidir o que fazer (exibir msg "sem permissão", etc.)
                throw new UnauthorizedAccessException("Sem permissão para consultar status.");
            }

            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadFromJsonAsync<List<StatusDto>>(cancellationToken: ct)
                   ?? new();
        }

        public async Task<StatusDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            AttachAuthHeader();

            var resp = await SendAsync(c => c.GetAsync($"api/statuses/{id}", ct));
            Console.WriteLine($"[StatusesService] Status GET /api/statuses/{id} = {(int)resp.StatusCode}");

            if (resp.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Sem permissão para consultar status.");

            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadFromJsonAsync<StatusDto>(cancellationToken: ct);
        }

        public async Task<StatusDto> CreateAsync(CreateStatusDto dto, CancellationToken ct = default)
        {
            AttachAuthHeader();

            var resp = await SendAsync(c => c.PostAsJsonAsync("api/statuses", dto, ct));
            var body = await resp.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"[StatusesService] Status POST /api/statuses = {(int)resp.StatusCode}");

            if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Sem permissão para criar status.");

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao criar status ({(int)resp.StatusCode}): {body}");

            return (await resp.Content.ReadFromJsonAsync<StatusDto>(cancellationToken: ct))!;
        }

        public async Task<StatusDto> UpdateAsync(int id, UpdateStatusDto dto, CancellationToken ct = default)
        {
            AttachAuthHeader();

            var resp = await SendAsync(c => c.PutAsJsonAsync($"api/statuses/{id}", dto, ct));
            var body = await resp.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"[StatusesService] Status PUT /api/statuses/{id} = {(int)resp.StatusCode}");

            if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Sem permissão para atualizar status.");

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao atualizar status ({(int)resp.StatusCode}): {body}");

            return (await resp.Content.ReadFromJsonAsync<StatusDto>(cancellationToken: ct))!;
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            AttachAuthHeader();

            var resp = await SendAsync(c => c.DeleteAsync($"api/statuses/{id}", ct));
            var body = await resp.Content.ReadAsStringAsync(ct);
            Console.WriteLine($"[StatusesService] Status DELETE /api/statuses/{id} = {(int)resp.StatusCode}");

            if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                throw new UnauthorizedAccessException("Sem permissão para excluir status.");

            if (!resp.IsSuccessStatusCode)
                throw new InvalidOperationException($"Falha ao excluir status ({(int)resp.StatusCode}): {body}");
        }
    }
}

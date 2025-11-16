using DockQueue.Application.DTOs.Permissions;
using DockQueue.Client.ViewModels;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DockQueue.Client.Services
{
    public interface IPermissionsApi
    {
        Task<PermissionsScreenDataDto?> GetScreenDataAsync(int userId, CancellationToken ct = default);
        Task<bool> UpdateAsync(int userId, UpdateOperatorPermissionsDto dto, CancellationToken ct = default);
    }

    public class PermissionsApi : IPermissionsApi
    {
        private readonly HttpClient _httpClient;
        private readonly AuthViewModel _auth;
        private readonly JwtAuthenticationStateProvider _authProvider;

        public PermissionsApi(IHttpClientFactory httpClientFactory, AuthViewModel auth,
            JwtAuthenticationStateProvider authProvider)
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

        public Task<PermissionsScreenDataDto?> GetScreenDataAsync(int userId, CancellationToken ct = default)
        {
            AttachAuthHeader();
            return _httpClient.GetFromJsonAsync<PermissionsScreenDataDto>(
                $"/api/operators/{userId}/permissions/screen-data", ct);
        }

        public async Task<bool> UpdateAsync(int userId, UpdateOperatorPermissionsDto dto, CancellationToken ct = default)
        {
            AttachAuthHeader();
            var resp =await SendAsync(c => c.PutAsJsonAsync($"/api/operators/{userId}/permissions", dto, ct));
            return resp.IsSuccessStatusCode;
        }
    }
}

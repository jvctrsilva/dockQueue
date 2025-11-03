namespace DockQueue.Client.Services;
using DockQueue.Application.DTOs.Permissions;
/// Consome a API de permissões de operador:
/// GET /api/operators/{userId}/permissions
/// PUT /api/operators/{userId}/permissions
// DockQueue.Client/Services/PermissionsApi.cs
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

public interface IPermissionsApi
{
    Task<PermissionsScreenDataDto?> GetScreenDataAsync(int userId, CancellationToken ct = default);
    Task<bool> UpdateAsync(int userId, UpdateOperatorPermissionsDto dto, CancellationToken ct = default);
}

public class PermissionsApi : IPermissionsApi
{
    private readonly HttpClient _http;

    // Pega o client nomeado "ApiClient"
    public PermissionsApi(IHttpClientFactory factory)
        => _http = factory.CreateClient("ApiClient");

    public Task<PermissionsScreenDataDto?> GetScreenDataAsync(int userId, CancellationToken ct = default)
        => _http.GetFromJsonAsync<PermissionsScreenDataDto>(
            $"api/operators/{userId}/permissions/screen-data", ct); // <- rota certa

    public async Task<bool> UpdateAsync(int userId, UpdateOperatorPermissionsDto dto, CancellationToken ct = default)
    {
        var resp = await _http.PutAsJsonAsync($"api/operators/{userId}/permissions", dto, ct);
        return resp.IsSuccessStatusCode;
    }
}


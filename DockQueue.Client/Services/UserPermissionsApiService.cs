using DockQueue.Application.DTOs.Permissions;
using DockQueue.Client.ViewModels;
using System.Net.Http.Headers;


namespace DockQueue.Client.Services;
public interface IPermissionsApi
{
    Task<PermissionsScreenDataDto?> GetScreenDataAsync(int userId, CancellationToken ct = default);
    Task<bool> UpdateAsync(int userId, UpdateOperatorPermissionsDto dto, CancellationToken ct = default);

}

public class PermissionsApi : IPermissionsApi
{
    private readonly HttpClient _httpClient;
    private readonly AuthViewModel _auth;

    // Pega o client nomeado "ApiClient"
    public PermissionsApi(IHttpClientFactory httpClientFactory, AuthViewModel auth)
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
    public Task<PermissionsScreenDataDto?> GetScreenDataAsync(int userId, CancellationToken ct = default)
    {
        AttachAuthHeader();
        return _httpClient.GetFromJsonAsync<PermissionsScreenDataDto>(
            $"api/operators/{userId}/permissions/screen-data", ct);
    }
    public async Task<bool> UpdateAsync(int userId, UpdateOperatorPermissionsDto dto, CancellationToken ct = default)
    {
        AttachAuthHeader();
        var resp = await _httpClient.PutAsJsonAsync($"api/operators/{userId}/permissions", dto, ct);
        return resp.IsSuccessStatusCode;
    }
}


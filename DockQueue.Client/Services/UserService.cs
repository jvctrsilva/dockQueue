using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using System.Net;
using System.Net.Http.Headers;

namespace DockQueue.Client.Services;
public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly AuthViewModel _auth;
    private readonly JwtAuthenticationStateProvider _authProvider;

    public UserService(IHttpClientFactory httpClientFactory, AuthViewModel auth,
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

    public async Task<List<UserDto>> GetAllAsync()
    {
        var response = await SendAsync(c => c.GetAsync("/api/users"));

        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        return users ?? new List<UserDto>();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var response = await SendAsync(c => c.GetAsync($"/api/users/{id}"));
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto?> CreateAsync(CreateUserDto dto)
    {
        var response = await SendAsync(c => c.PostAsJsonAsync("/api/users", dto));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
    {
        var response = await SendAsync(c => c.PutAsJsonAsync($"/api/users/{id}", dto));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await SendAsync(c => c.DeleteAsync($"/api/users/{id}"));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePasswordAsync(int id, UpdatePasswordDto dto)
    {
        var response = await SendAsync(c => c.PutAsJsonAsync($"/api/users/{id}/password", dto));
        return response.IsSuccessStatusCode;
    }
}


using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using System.Net;
using System.Net.Http.Headers;

namespace DockQueue.Client.Services;
public class UserService
{
    private readonly HttpClient _httpClient;
    private readonly AuthViewModel _auth;

    public UserService(IHttpClientFactory httpClientFactory, AuthViewModel auth)
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
    public async Task<List<UserDto>> GetAllAsync()
    {
        AttachAuthHeader();

        var response = await _httpClient.GetAsync("/api/users");

        // se não autorizado, deixa o chamador decidir o que fazer (ex.: redirecionar pro login)
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new UnauthorizedAccessException("API retornou 401 para /api/users.");

        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        return users ?? new List<UserDto>();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        AttachAuthHeader();
        var response = await _httpClient.GetAsync($"/api/users/{id}");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<UserDto?> CreateAsync(CreateUserDto dto)
    {
        AttachAuthHeader();
        var response = await _httpClient.PostAsJsonAsync("/api/users", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
    {
        AttachAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"/api/users/{id}", dto);
        return response.IsSuccessStatusCode;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        AttachAuthHeader();
        var response = await _httpClient.DeleteAsync($"/api/users/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdatePasswordAsync(int id, UpdatePasswordDto dto)
    {
        AttachAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"/api/users/{id}/password", dto);
        return response.IsSuccessStatusCode;
    }
}


using DockQueue.Application.DTOs;

namespace DockQueue.Client.Services;
public class UserService
{
    private readonly HttpClient _httpClient;

    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }
    public async Task<List<UserDto>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync("/api/users");
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        return users ?? new List<UserDto>();
    }

    public async Task<UserDto?> CreateAsync(CreateUserDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/users", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<UserDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/users/{id}");
        return response.IsSuccessStatusCode;
    }
}


using System.Net.Http.Headers;
using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;

public class BoxService
{
    private readonly HttpClient _httpClient;
    private readonly AuthViewModel _auth;

    public BoxService(IHttpClientFactory httpClientFactory, AuthViewModel auth)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _auth = auth;
    }

    private void AttachAuthHeader()
    {
        var token = _auth.AccessToken;
        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            // Se quiser limpar caso esteja vazio:
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<BoxDto?> GetByIdAsync(int id)
    {
        AttachAuthHeader();
        var response = await _httpClient.GetAsync($"/api/box/{id}");
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<BoxDto>();
    }

    public async Task<List<BoxDto>> GetAllAsync()
    {
        AttachAuthHeader();

        var response = await _httpClient.GetAsync("/api/box");
        response.EnsureSuccessStatusCode();

        var boxes = await response.Content.ReadFromJsonAsync<List<BoxDto>>();
        return boxes ?? new List<BoxDto>();
    }

    public async Task<BoxDto?> CreateAsync(CreateBoxDto dto)
    {
        AttachAuthHeader();

        var response = await _httpClient.PostAsJsonAsync("/api/box", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BoxDto>();
    }

    public async Task<BoxDto?> UpdateAsync(int id, UpdateBoxDto dto)
    {
        AttachAuthHeader();
        var response = await _httpClient.PutAsJsonAsync($"/api/box/{id}", dto);
        if (!response.IsSuccessStatusCode)
            return null;
        return await response.Content.ReadFromJsonAsync<BoxDto>();
    }
    public async Task<bool> DeleteAsync(int id)
    {
        AttachAuthHeader();

        var response = await _httpClient.DeleteAsync($"/api/box/{id}");
        return response.IsSuccessStatusCode;
    }
}

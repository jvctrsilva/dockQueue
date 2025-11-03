using System.Net.Http.Json;
using DockQueue.Application.DTOs;

namespace DockQueue.Client.Services;

/// <summary>
/// Responsável por consumir o BoxController da API.
/// </summary>
public class BoxService
{
    private readonly HttpClient _httpClient;

    public BoxService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    /// <summary>
    /// Busca todos os boxes cadastrados na API.
    /// </summary>
    public async Task<List<BoxDto>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync("/api/box");
        response.EnsureSuccessStatusCode();

        var boxes = await response.Content.ReadFromJsonAsync<List<BoxDto>>();
        return boxes ?? new List<BoxDto>();
    }

    /// <summary>
    /// Cria um novo box.
    /// </summary>
    public async Task<BoxDto?> CreateAsync(CreateBoxDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/box", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BoxDto>();
    }

    /// <summary>
    /// Remove um box pelo ID.
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/box/{id}");
        return response.IsSuccessStatusCode;
    }
}

using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using System.Net;
using System.Net.Http.Headers;

public class BoxService
{
    private readonly HttpClient _httpClient;
    private readonly AuthViewModel _auth;
    private readonly JwtAuthenticationStateProvider _authProvider;

    public BoxService(IHttpClientFactory httpClientFactory, AuthViewModel auth, JwtAuthenticationStateProvider authProvider)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _auth = auth;
        _authProvider = authProvider;
    }

    private void AttachAuthHeader()
    {
        var token = _auth.AccessToken;
        Console.WriteLine($"[BoxService] AttachAuthHeader - token length = {token?.Length ?? 0}");
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


    public async Task<BoxDto?> GetByIdAsync(int id)
    {
        AttachAuthHeader();
        var response = await SendAsync(c => c.GetAsync("/api/box"));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BoxDto>();
    }

    public async Task<List<BoxDto>> GetAllAsync()
    {
        AttachAuthHeader();

        var response = await SendAsync(c => c.GetAsync("/api/box"));
        response.EnsureSuccessStatusCode();

        var boxes = await response.Content.ReadFromJsonAsync<List<BoxDto>>();
        return boxes ?? new List<BoxDto>();
    }

    public async Task<BoxDto?> CreateAsync(CreateBoxDto dto)
    {
        AttachAuthHeader();

        var response = await SendAsync(c => c.PostAsJsonAsync("/api/box", dto));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BoxDto>();
    }

    public async Task<BoxDto?> UpdateAsync(int id, UpdateBoxDto dto)
    {
        AttachAuthHeader();

        var response = await SendAsync(c => c.PutAsJsonAsync($"/api/box/{id}", dto));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<BoxDto>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        AttachAuthHeader();

        var response = await SendAsync(c => c.DeleteAsync($"/api/box/{id}"));
        return response.IsSuccessStatusCode;
    }
}

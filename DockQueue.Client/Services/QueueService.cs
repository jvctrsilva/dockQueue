using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using DockQueue.Domain.Enums;
using System.Net.Http.Headers;

public class QueueService
{
    private readonly HttpClient _httpClient;
    private readonly AuthViewModel _auth;

    public QueueService(IHttpClientFactory httpClientFactory, AuthViewModel auth)
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
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<List<QueueEntryViewDto>> GetQueueAsync(QueueType type)
    {
        AttachAuthHeader();

        var response = await _httpClient.GetAsync($"/api/queue?type={type}");
        response.EnsureSuccessStatusCode();

        var entries = await response.Content.ReadFromJsonAsync<List<QueueEntryViewDto>>();
        return entries ?? new List<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> EnqueueAsync(CreateQueueEntryDto dto)
    {
        AttachAuthHeader();

        var response = await _httpClient.PostAsJsonAsync("/api/queue", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> UpdateStatusAsync(UpdateQueueEntryStatusDto dto)
    {
        AttachAuthHeader();

        var response = await _httpClient.PutAsJsonAsync("/api/queue/status", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> AssignBoxAsync(AssignBoxDto dto)
    {
        AttachAuthHeader();

        var response = await _httpClient.PutAsJsonAsync("/api/queue/assign-box", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> LookupAsync(DriverQueueLookupDto dto)
    {
        AttachAuthHeader();
        // se esse endpoint é AllowAnonymous, pode não anexar header aqui
        var response = await _httpClient.PostAsJsonAsync("/api/queue/lookup", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<bool> ClearQueueAsync(QueueType type)
    {
        AttachAuthHeader();

        var response = await _httpClient.DeleteAsync($"/api/queue?type={type}");
        return response.IsSuccessStatusCode;
    }
}

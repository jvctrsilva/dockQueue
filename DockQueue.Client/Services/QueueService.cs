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
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Falha ao atualizar status ({(int)response.StatusCode}): {ExtractErrorMessage(errorBody)}");
        }

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    private string ExtractErrorMessage(string errorBody)
    {
        if (string.IsNullOrWhiteSpace(errorBody))
            return "Erro desconhecido";

        try
        {
            var trimmed = errorBody.Trim();
            
            if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
            {
                using var doc = System.Text.Json.JsonDocument.Parse(trimmed);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("error", out var errorProp))
                {
                    var errorValue = errorProp.GetString();
                    if (!string.IsNullOrWhiteSpace(errorValue))
                        return errorValue;
                }
                
                if (root.TryGetProperty("message", out var messageProp))
                {
                    var messageValue = messageProp.GetString();
                    if (!string.IsNullOrWhiteSpace(messageValue))
                        return messageValue;
                }
                
                if (root.TryGetProperty("title", out var titleProp))
                {
                    var titleValue = titleProp.GetString();
                    if (!string.IsNullOrWhiteSpace(titleValue))
                        return titleValue;
                }
            }
            
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                return trimmed.Substring(1, trimmed.Length - 2);
            
            return trimmed;
        }
        catch
        {
            var trimmed = errorBody.Trim();
            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                return trimmed.Substring(1, trimmed.Length - 2);
            return trimmed;
        }
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

    public async Task<QueueEntryViewDto?> StartBoxOperationAsync(StartBoxOperationDto dto)
    {
        AttachAuthHeader();

        var response = await _httpClient.PostAsJsonAsync("/api/queue/start-box-operation", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> FinishBoxOperationAsync(FinishBoxOperationDto dto)
    {
        AttachAuthHeader();

        var response = await _httpClient.PostAsJsonAsync("/api/queue/finish-box-operation", dto);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }
}

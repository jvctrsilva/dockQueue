using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using DockQueue.Domain.Enums;
using System.Net;
using System.Net.Http.Headers;

public class QueueService
{
    private readonly HttpClient _httpClient;
    private readonly AuthViewModel _auth;
    private readonly JwtAuthenticationStateProvider _authProvider;

    public QueueService(IHttpClientFactory httpClientFactory, AuthViewModel auth, JwtAuthenticationStateProvider authProvider)
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

    public async Task<List<QueueEntryViewDto>> GetQueueAsync(QueueType type)
    {
        AttachAuthHeader();
        var response = await SendAsync(c => c.GetAsync($"/api/queue?type={type}"));
        response.EnsureSuccessStatusCode();

        var entries = await response.Content.ReadFromJsonAsync<List<QueueEntryViewDto>>();
        return entries ?? new List<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> EnqueueAsync(CreateQueueEntryDto dto, CancellationToken ct = default)
    {
        var resp = await SendAsync(c => c.PostAsJsonAsync("/api/queue", dto, ct));

        if (!resp.IsSuccessStatusCode)
            return null;

        return await resp.Content.ReadFromJsonAsync<QueueEntryViewDto>(cancellationToken: ct);
    }

    public async Task<QueueEntryViewDto?> UpdateStatusAsync(UpdateQueueEntryStatusDto dto)
    {
        AttachAuthHeader();
        var response = await SendAsync(c => c.PutAsJsonAsync("/api/queue/status", dto));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> AssignBoxAsync(AssignBoxDto dto)
    {
        AttachAuthHeader();
        var response = await SendAsync(c => c.PutAsJsonAsync("/api/queue/assign-box", dto));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> LookupAsync(DriverQueueLookupDto dto)
    {
        AttachAuthHeader();
        var response = await SendAsync(c => c.PostAsJsonAsync("/api/queue/lookup", dto));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<bool> ClearQueueAsync(QueueType type)
    {
        AttachAuthHeader();
        var response = await SendAsync(c => c.DeleteAsync($"/api/queue?type={type}"));
        return response.IsSuccessStatusCode;
    }

    public async Task<QueueEntryViewDto?> StartBoxOperationAsync(StartBoxOperationDto dto)
    {
        AttachAuthHeader();
        var response = await SendAsync(c => c.PostAsJsonAsync("/api/queue/start-box-operation", dto));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }

    public async Task<QueueEntryViewDto?> FinishBoxOperationAsync(FinishBoxOperationDto dto)
    {
        var response = await SendAsync(c => c.PostAsJsonAsync("/api/queue/finish-box-operation", dto));
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<QueueEntryViewDto>();
    }
}

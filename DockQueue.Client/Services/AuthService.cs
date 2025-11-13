using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using System.Text;
using System.Text.Json;

namespace DockQueue.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly JwtAuthenticationStateProvider _authProvider;

    public AuthService(
        IHttpClientFactory httpClientFactory,
        JwtAuthenticationStateProvider authProvider)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _authProvider = authProvider;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginViewModel loginViewModel)
    {
        try
        {
            var loginDto = loginViewModel.ToDto();
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/login", content);

            if (!response.IsSuccessStatusCode)
            {
                loginViewModel.SetError("Email ou senha inválidos");
                return null;
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (authResponse is null || string.IsNullOrWhiteSpace(authResponse.Token))
            {
                loginViewModel.SetError("Resposta de autenticação inválida.");
                return null;
            }

            Console.WriteLine($"[AuthService] Token recebido com {authResponse.Token.Length} caracteres");

            // Salva em localStorage + atualiza AuthViewModel + notifica Blazor
            await _authProvider.SetAuthDataAsync(authResponse);

            return authResponse;
        }
        catch (Exception ex)
        {
            loginViewModel.SetError($"Erro ao fazer login: {ex.Message}");
            return null;
        }
    }

    public async Task LogoutAsync()
    {
        await _authProvider.ClearAuthDataAsync();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var stored = await _authProvider.GetStoredAuthDataAsync();
        return stored is not null && !string.IsNullOrWhiteSpace(stored.Token);
    }
}

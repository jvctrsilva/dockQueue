using DockQueue.Application.DTOs;
using DockQueue.Client.Services.UI;
using DockQueue.Client.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text;
using System.Text.Json;

namespace DockQueue.Client.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthViewModel _authViewModel;
    private readonly SessionService _sessionService;
    private readonly AuthenticationStateProvider _authProvider;

    public AuthService(IHttpClientFactory httpClientFactory, AuthViewModel authViewModel, SessionService sessionService, AuthenticationStateProvider authProvider)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _authViewModel = authViewModel;
        _sessionService = sessionService;
        _authProvider = authProvider;

    }

    public async Task<bool> LoginAsync(LoginViewModel loginViewModel)
    {
        try
        {
            var loginDto = loginViewModel.ToDto();
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<AuthResponseDto>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (authResponse != null)
                {
                    _authViewModel.SetAuthData(authResponse);
                    await SaveAuthToSession(authResponse);

                    // **força recomputar claims no Blazor**
                    (_authProvider as JwtAuthenticationStateProvider)?.Refresh();
                    return true;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                loginViewModel.SetError("Email ou senha inválidos");
            }
        }
        catch (Exception ex)
        {
            loginViewModel.SetError($"Erro ao fazer login: {ex.Message}");
        }

        return false;
    }

    public async Task LogoutAsync()
    {
        _authViewModel.ClearAuthData();
        await ClearAuthFromSession();

        (_authProvider as JwtAuthenticationStateProvider)?.Refresh();
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var authData = await GetAuthFromSession();
        if (authData != null)
        {
            _authViewModel.SetAuthData(authData);
            return _authViewModel.IsAuthenticated;
        }
        return false;
    }

    private async Task SaveAuthToSession(AuthResponseDto authResponse)
    {
        try
        {
            var json = JsonSerializer.Serialize(authResponse);
            var httpContext = _sessionService.GetHttpContext();
            if (httpContext?.Session != null)
            {
                httpContext.Session.SetString("AuthData", json);
            }
            await Task.CompletedTask;
        }
        catch (Exception)
        {
            // Log error if needed
        }
    }

    private async Task<AuthResponseDto?> GetAuthFromSession()
    {
        try
        {
            var httpContext = _sessionService.GetHttpContext();
            if (httpContext?.Session != null)
            {
                var json = httpContext.Session.GetString("AuthData");
                if (!string.IsNullOrEmpty(json))
                {
                    return JsonSerializer.Deserialize<AuthResponseDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            await Task.CompletedTask;
        }
        catch (Exception)
        {
            // Log error if needed
        }
        return null;
    }

    private async Task ClearAuthFromSession()
    {
        try
        {
            var httpContext = _sessionService.GetHttpContext();
            if (httpContext?.Session != null)
            {
                httpContext.Session.Remove("AuthData");
            }
            await Task.CompletedTask;
        }
        catch (Exception)
        {
            // Log error if needed
        }
    }
}

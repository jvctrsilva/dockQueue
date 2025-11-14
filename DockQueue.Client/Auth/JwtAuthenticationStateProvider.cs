using System.Security.Claims;
using System.Security.Cryptography;
using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthViewModel _auth;
    private readonly IHttpClientFactory _httpClientFactory;

    public JwtAuthenticationStateProvider(ProtectedLocalStorage localStorage, AuthViewModel auth, IHttpClientFactory httpClientFactory)
    {
        _localStorage = localStorage;
        _auth = auth;
        _httpClientFactory = httpClientFactory;

    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("[AuthProvider] GetAuthenticationStateAsync INICIO");

        try
        {
            var stored = await _localStorage.GetAsync<AuthResponseDto>("AuthData");
            Console.WriteLine($"[AuthProvider] ProtectedLocalStorage.Success = {stored.Success}");

            var authData = stored.Success ? stored.Value : null;

            if (authData is null || string.IsNullOrWhiteSpace(authData.Token))
            {
                Console.WriteLine("[AuthProvider] Sem AuthData ou token vazio -> Anon");
                await _localStorage.DeleteAsync("AuthData");
                _auth.ClearAuthData();
                return Anon();
            }
            Console.WriteLine($"[AuthProvider] Token length = {authData.Token.Length}");


            // 1) Verifica expiração do token atual
            if (JwtHelper.IsExpired(authData.Token))
            {
                // 2) Tenta refresh
                authData = await TryRefreshAsync(authData);
                if (authData is null)
                {
                    // refresh falhou → limpa tudo e trata como anônimo
                    await _localStorage.DeleteAsync("AuthData");
                    _auth.ClearAuthData();
                    return Anon();
                }
            }

            // 3) Nesse ponto authData tem token válido (original ou refresh)
            _auth.SetAuthData(authData);

            var claims = JwtHelper.ExtractClaims(authData.Token);
            var identity = new ClaimsIdentity(claims, "jwt");
            Console.WriteLine("[AuthProvider] GetAuthenticationStateAsync OK (usuario autenticado)");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch(Exception ex)

        {
            Console.WriteLine("[AuthProvider] ERRO em GetAuthenticationStateAsync: " + ex);

            _auth.ClearAuthData();
            return Anon();
        }
    }


    public async Task SetAuthDataAsync(AuthResponseDto authData)
    {
        Console.WriteLine("[AuthProvider] Limpando AuthData do storage");
        await _localStorage.SetAsync("AuthData", authData);
        _auth.SetAuthData(authData);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<AuthResponseDto?> GetStoredAuthDataAsync()
    {
        try
        {
            var result = await _localStorage.GetAsync<AuthResponseDto>("AuthData");

            if (result.Success && result.Value is not null)
                return result.Value;

            return null;
        }
        catch (CryptographicException)
        {
            // Dados não conseguem ser descriptografados -> limpa e trata como deslogado
            await _localStorage.DeleteAsync("AuthData");
            return null;
        }
    }

    // Chamar apenas de lugares onde JS está disponível (componentes, AuthService/logout, etc.)
    public async Task ClearAuthDataAsync()
    {
        Console.WriteLine("[AuthProvider] Limpando AuthData do storage");
        await _localStorage.DeleteAsync("AuthData");
        ClearAuthDataInMemory();
    }

    public void ClearAuthDataInMemory()
    {
        _auth.ClearAuthData();

        // Não chama GetAuthenticationStateAsync (que mexe com storage), 
        var anonState = Task.FromResult(Anon());
        NotifyAuthenticationStateChanged(anonState);
    }

    private static AuthenticationState Anon()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));

    private async Task<AuthResponseDto?> TryRefreshAsync(AuthResponseDto authData)
    {
        // se não tiver refresh token, não dá pra fazer nada
        if (string.IsNullOrWhiteSpace(authData.RefreshToken))
            return null;

        var client = _httpClientFactory.CreateClient("ApiClient");

        var request = new RefreshTokenResponseDto
        {
            AccessToken = authData.Token,
            RefreshToken = authData.RefreshToken
        };

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsJsonAsync("/api/refresh", request);
        }
        catch
        {
            // se não conseguir falar com API (offline / erro), não atualiza
            return null;
        }

        if (!response.IsSuccessStatusCode)
            return null;

        var refreshResponse = await response.Content.ReadFromJsonAsync<RefreshTokenResponseDto>();
        if (refreshResponse is null || string.IsNullOrWhiteSpace(refreshResponse.AccessToken))
            return null;

        // Atualiza o AuthResponse em memória
        authData.Token = refreshResponse.AccessToken;
        authData.RefreshToken = refreshResponse.RefreshToken;

        // Persiste no storage + atualiza VM
        await _localStorage.SetAsync("AuthData", authData);
        _auth.SetAuthData(authData);

        return authData;
    }

}

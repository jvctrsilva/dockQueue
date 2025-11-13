using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DockQueue.Application.DTOs;
using DockQueue.Client.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly AuthViewModel _auth;

    public JwtAuthenticationStateProvider(
        ProtectedLocalStorage localStorage,
        AuthViewModel auth)
    {
        _localStorage = localStorage;
        _auth = auth;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var stored = await _localStorage.GetAsync<AuthResponseDto>("AuthData");
            var authData = stored.Success ? stored.Value : null;

            if (authData is null || string.IsNullOrWhiteSpace(authData.Token))
            {
                _auth.ClearAuthData();
                return Anon();
            }

            // Atualiza o AuthViewModel (token para os services)
            _auth.SetAuthData(authData);

            // Monta as claims para o Blazor
            var claims = JwtHelper.ExtractClaims(authData.Token);
            var identity = new ClaimsIdentity(claims, "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            _auth.ClearAuthData();
            return Anon();
        }
    }

    public async Task SetAuthDataAsync(AuthResponseDto authData)
    {
        await _localStorage.SetAsync("AuthData", authData);
        _auth.SetAuthData(authData);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<AuthResponseDto?> GetStoredAuthDataAsync()
    {
        var stored = await _localStorage.GetAsync<AuthResponseDto>("AuthData");
        return stored.Success ? stored.Value : null;
    }

    public async Task ClearAuthDataAsync()
    {
        await _localStorage.DeleteAsync("AuthData");
        _auth.ClearAuthData();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static AuthenticationState Anon()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));
}

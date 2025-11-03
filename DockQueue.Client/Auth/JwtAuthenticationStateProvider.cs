using DockQueue.Application.DTOs;
using DockQueue.Client.Services.UI;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly SessionService _session;

    public JwtAuthenticationStateProvider(SessionService session) => _session = session;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var http = _session.GetHttpContext();
        var json = http?.Session?.GetString("AuthData");

        if (string.IsNullOrEmpty(json))
            return Task.FromResult(Anon());

        var auth = JsonSerializer.Deserialize<AuthResponseDto>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (auth is null || string.IsNullOrWhiteSpace(auth.Token))
            return Task.FromResult(Anon());

        try
        {
            var claims = JwtHelper.ExtractClaims(auth.Token);
            var identity = new ClaimsIdentity(claims, "jwt");
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
        catch
        {
            return Task.FromResult(Anon());
        }
    }

    public void Refresh()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

    private static AuthenticationState Anon()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));
}

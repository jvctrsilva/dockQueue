// Handlers/AuthMessageHandler.cs
using System.Net.Http.Headers;
using DockQueue.Client.Services.UI;
using DockQueue.Application.DTOs;
using System.Text.Json;

namespace DockQueue.Client.Handlers
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly SessionService _sessionService;
        public AuthMessageHandler(SessionService sessionService) => _sessionService = sessionService;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var httpContext = _sessionService.GetHttpContext();
            var json = httpContext?.Session?.GetString("AuthData");

            

            if (!string.IsNullOrEmpty(json))
            {
                var auth = JsonSerializer.Deserialize<AuthResponseDto>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // suporta ambos os nomes, se no futuro você padronizar para AccessToken
                var token = auth?.Token;
                if (string.IsNullOrWhiteSpace(token))
                {
                    // fallback caso mude o contrato depois
                    var accessTokenProp = auth?.GetType().GetProperty("AccessToken");
                    token = accessTokenProp?.GetValue(auth)?.ToString();
                }

                if (!string.IsNullOrWhiteSpace(token))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            Console.WriteLine($"[AuthHandler] Authorization presente? {request.Headers.Authorization != null}");

            return await base.SendAsync(request, ct);
        }
    }
}
// Handlers/AuthMessageHandler.cs
using System.Net.Http.Headers;
using DockQueue.Client.ViewModels;

namespace DockQueue.Client.Handlers
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly AuthViewModel _authViewModel;

        public AuthMessageHandler(AuthViewModel authViewModel)
        {
            _authViewModel = authViewModel;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var token = _authViewModel.AccessToken;

            Console.WriteLine($"[AuthHandler] AccessToken no VM length = {token?.Length ?? 0}");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, ct);
        }
    }
}

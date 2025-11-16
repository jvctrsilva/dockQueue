using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using DockQueue.Client.ViewModels;

namespace DockQueue.Client.Handlers
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly AuthViewModel _authViewModel;
        private readonly JwtAuthenticationStateProvider _authProvider;

        public AuthMessageHandler(
            AuthViewModel authViewModel,
            JwtAuthenticationStateProvider authProvider)
        {
            _authViewModel = authViewModel;
            _authProvider = authProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken ct)
        {
            var path = request.RequestUri?.AbsolutePath ?? string.Empty;

            // 1) Anexa o token se existir
            var token = _authViewModel.AccessToken;
            Console.WriteLine($"[AuthHandler] Request {path} | Token length = {token?.Length ?? 0}");

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"[AuthHandler] Authorization header set for {path}");

            }
            else
            {
                Console.WriteLine($"[AuthHandler] SEM token para {path}");

            }

            var response = await base.SendAsync(request, ct);

            Console.WriteLine($"[AuthHandler] Response {path} -> {(int)response.StatusCode}");

            // 2) Verifica se é endpoint de auth (login/refresh)
            var isAuthEndpoint =
                path.Equals("/api/login", StringComparison.OrdinalIgnoreCase) ||
                path.Equals("/api/refresh", StringComparison.OrdinalIgnoreCase);

            // 3) Para qualquer 401 fora de login/refresh, limpamos auth em memória
            if (!isAuthEndpoint && response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _authProvider.ClearAuthDataInMemory();
                // sem NavigationManager aqui. Redirect fica a cargo do App.razor (RedirectToLogin)
            }

            // 4) 403: por enquanto não navegamos aqui. 
            // Você pode tratar 403 nas páginas/Services se quiser mandar para /acesso-negado.

            return response;
        }
    }
}

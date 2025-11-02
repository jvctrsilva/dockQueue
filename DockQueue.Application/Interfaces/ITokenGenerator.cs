using System.Security.Claims;

namespace DockQueue.Application.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(string userId, string email, string role, IDictionary<string, string>? extraClaims = null);
        string GenerateAccessToken(string userId, string email);
        (string token, DateTime expires) GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}


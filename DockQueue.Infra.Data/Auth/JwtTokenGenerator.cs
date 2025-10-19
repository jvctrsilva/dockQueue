using DockQueue.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DockQueue.Infra.Data.Auth
{
    public class JwtTokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private (SymmetricSecurityKey key, string issuer, string audience, double accessMinutes, int refreshDays) GetJwtSettings()
        {
            var jwt = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"] ?? throw new Exception("JWT Key não configurada")));
            var issuer = jwt["Issuer"]!;
            var audience = jwt["Audience"]!;
            var accessMinutes = Convert.ToDouble(jwt["AccessTokenMinutes"] ?? "30");
            var refreshDays = Convert.ToInt32(jwt["RefreshTokenDays"] ?? "7");
            return (key, issuer, audience, accessMinutes, refreshDays);
        }

        public string GenerateAccessToken(string userId, string email, string role)
        {
            var (key, issuer, audience, accessMinutes, _) = GetJwtSettings();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Email, email), // <- garante que ClaimTypes.Email exista
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(issuer, audience, claims,
                expires: DateTime.UtcNow.AddMinutes(accessMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateAccessToken(string userId, string email)
        {
            var (key, issuer, audience, accessMinutes, _) = GetJwtSettings();
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(ClaimTypes.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(issuer, audience, claims,
                expires: DateTime.UtcNow.AddMinutes(accessMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string token, DateTime expires) GenerateRefreshToken()
        {
            var (_, _, _, _, refreshDays) = GetJwtSettings();
            var bytes = RandomNumberGenerator.GetBytes(64);
            return (Convert.ToBase64String(bytes), DateTime.UtcNow.AddDays(refreshDays));
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var (key, _, _, _, _) = GetJwtSettings();

            var parameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = false
            };

            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, parameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwt ||
                !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}

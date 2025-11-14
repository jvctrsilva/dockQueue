using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class JwtHelper
{
    public static IEnumerable<Claim> ExtractClaims(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        var claims = new List<Claim>();
        claims.AddRange(token.Claims);

        if (token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier) is null)
        {
            var sub = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (!string.IsNullOrWhiteSpace(sub))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
        }

        if (token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) is null)
        {
            var email = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim(ClaimTypes.Email, email));
        }

        var hasRoleClaimTypes = token.Claims.Any(c => c.Type == ClaimTypes.Role);
        var roleRaw = token.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        if (!hasRoleClaimTypes && !string.IsNullOrWhiteSpace(roleRaw))
        {
            foreach (var r in roleRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                claims.Add(new Claim(ClaimTypes.Role, r));
        }

        return claims;
    }

    /// <summary>
    /// Retorna a data de expiração (UTC) do token, se existir.
    /// </summary>
    public static DateTime? GetExpirationUtc(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        // JwtSecurityToken já resolve o 'exp' para ValidTo (UTC)
        return token.ValidTo;
    }

    /// <summary>
    /// Retorna true se o token já estiver expirado em relação ao UtcNow.
    /// </summary>
    public static bool IsExpired(string jwt, DateTime? nowUtc = null)
    {
        nowUtc ??= DateTime.UtcNow;
        var exp = GetExpirationUtc(jwt);
        if (exp is null) return false; // se não tiver exp, não assumimos expirado aqui
        return exp <= nowUtc;
    }
}

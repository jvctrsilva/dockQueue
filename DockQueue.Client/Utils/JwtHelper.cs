// Utils/JwtHelper.cs (no projeto Blazor)
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public static class JwtHelper
{
    /// <summary>
    /// Lê o JWT e retorna uma lista de Claims pronta para montar o ClaimsPrincipal no Blazor.
    /// Não faz validação de assinatura/expiração – é apenas parse local.
    /// </summary>
    public static IEnumerable<Claim> ExtractClaims(string jwt)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt); // joga exceção se o token for malformado

        var claims = new List<Claim>();

        // Copia tudo que já veio no token
        claims.AddRange(token.Claims);

        // Normalizações úteis:
        // 1) sub -> NameIdentifier (se não existir)
        if (token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier) is null)
        {
            var sub = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            if (!string.IsNullOrWhiteSpace(sub))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, sub));
        }

        // 2) email -> ClaimTypes.Email (se não existir)
        if (token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) is null)
        {
            var email = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
            if (!string.IsNullOrWhiteSpace(email))
                claims.Add(new Claim(ClaimTypes.Email, email));
        }

        // 3) role mapeado para ClaimTypes.Role (caso só exista como "role" simples)
        var hasRoleClaimTypes = token.Claims.Any(c => c.Type == ClaimTypes.Role);
        var roleRaw = token.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        if (!hasRoleClaimTypes && !string.IsNullOrWhiteSpace(roleRaw))
        {
            // se vier "Admin,Manager" você pode splitar:
            foreach (var r in roleRaw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                claims.Add(new Claim(ClaimTypes.Role, r));
        }

        // 4) screens já vem como string/int no token (ex.: "127")
        // nada especial aqui, apenas garantimos que a claim exista.
        // (Se quiser renomear, faça aqui.)

        return claims;
    }
}

// API/Controllers/LoginController.cs
using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class LoginController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IOperatorPermissionsService _permService;

    public LoginController(
        IUserService userService,
        ITokenGenerator tokenGenerator,
        IOperatorPermissionsService permService)
    {
        _userService = userService;
        _tokenGenerator = tokenGenerator;
        _permService = permService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto, CancellationToken ct = default)
    {
        var user = await _userService.AuthenticateAsync(loginUserDto);
        if (user is null) return Unauthorized("Email ou senha inválidos");

        // Verifica se usuário precisa definir senha (primeiro login)
        var requiresPasswordChange = await _userService.RequiresPasswordChangeAsync(user.Id);

        // Se precisa definir senha, permite login mas indica que deve definir senha
        // O frontend deve redirecionar para página de definição de senha

        // 1) carrega permissões do usuário
        var perms = await _permService.GetByUserIdAsync(user.Id, ct);
        var isAdmin = string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);

        // 2) admin tem tudo; senão usa o que veio do banco
        var effectiveScreens = isAdmin ? Screen.All : (perms?.AllowedScreens ?? Screen.None);

        // 3) coloca claims extras (ESSENCIAL para o front e policies)
        var extraClaims = new Dictionary<string, string>
        {
            ["screens"] = ((int)effectiveScreens).ToString(),
            ["uid"] = user.Id.ToString(),
            ["role"] = user.Role ?? "User"
        };

        var accessToken = _tokenGenerator.GenerateAccessToken(
            user.Id.ToString(),
            user.Email,
            user.Role ?? "User",
            extraClaims
        );

        var (refreshToken, expiry) = _tokenGenerator.GenerateRefreshToken();
        await _userService.UpdateRefreshTokenAsync(user.Id, refreshToken, expiry);

        return Ok(new AuthResponseDto
        {
            Token = accessToken,
            RefreshToken = refreshToken,
            User = user,
            RequiresPasswordChange = requiresPasswordChange
        });
    }
}

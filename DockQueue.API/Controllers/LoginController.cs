using Microsoft.AspNetCore.Mvc;
using DockQueue.Application.Interfaces;
using DockQueue.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DockQueue.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenGenerator _tokenGenerator;

        public LoginController(IUserService userService, ITokenGenerator tokenGenerator)
        {
            _userService = userService;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            var user = await _userService.AuthenticateAsync(loginUserDto);
            if (user == null)
                return Unauthorized("Email ou senha inválidos");

            var accessToken = _tokenGenerator.GenerateAccessToken(user.Id.ToString(), user.Email, user.Role);
            var (refreshToken, expiry) = _tokenGenerator.GenerateRefreshToken();

            await _userService.UpdateRefreshTokenAsync(user.Id, refreshToken, expiry);

            return Ok(new AuthResponseDto
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                User = user
            });
        }
    }
}

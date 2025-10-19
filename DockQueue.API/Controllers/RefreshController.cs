using Microsoft.AspNetCore.Mvc;
using DockQueue.Application.Interfaces;
using DockQueue.Application.DTOs;

namespace DockQueue.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RefreshController : ControllerBase
    {
        private readonly IUserService _userService;

        public RefreshController(IUserService userService) => _userService = userService;

        [HttpPost]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var rotated = await _userService.RotateRefreshTokenAsync(request.RefreshToken, request.AccessToken);
            if (rotated is null) return Unauthorized("Refresh inválido ou expirado");

            var (accessToken, newRefresh) = rotated.Value;
            return Ok(new { accessToken, refreshToken = newRefresh });
        }
    }
}

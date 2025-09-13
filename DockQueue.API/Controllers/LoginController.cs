using Microsoft.AspNetCore.Mvc;    
using DockQueue.Application.Interfaces; 
using DockQueue.Application.DTOs;

namespace DockQueue.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _userService.AuthenticateAsync(loginUserDto);

            if (user == null)
                return Unauthorized("Email ou senha inválidos");

            return Ok(user);
        }
    }
}

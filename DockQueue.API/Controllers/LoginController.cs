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


        public LoginController(IUserService userService, ITokenGenerator jwtTokenGenerator)
        {
            _userService = userService;
            _tokenGenerator = jwtTokenGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto loginUserDto)
        {
            var user = await _userService.AuthenticateAsync(loginUserDto);

            if (user == null)
                return Unauthorized("Email ou senha inválidos");

            //gera token JWT
            var token = _tokenGenerator.GenerateToken(user.Id.ToString(), user.Email, user.Role);

            return Ok(new
            {
                token,
                user
            });
        }
    }
}

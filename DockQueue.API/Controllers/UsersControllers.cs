using Microsoft.AspNetCore.Mvc;
using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;

namespace DockQueue.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateUserDto createdUserDto)
        {
            var user = await _userService.CreateUserAsync(createdUserDto);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }
    }
}

using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DockQueue.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "Screen:UsersView")]
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateUserDto updateUserDto)
        {
            await _userService.UpdateUserAsync(id, updateUserDto);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateUserDto createdUserDto)
        {
            var user = await _userService.CreateUserAsync(createdUserDto);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }
        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/password")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordDto dto)
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return Unauthorized("Usuário não autenticado.");
                }

                // Verifica se o usuário está atualizando sua própria senha
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "uid") 
                               ?? User.Claims.FirstOrDefault(c => c.Type == "sub")
                               ?? User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                {
                    if (userId != id)
                    {
                        return Forbid("Você só pode atualizar sua própria senha.");
                    }
                }
                else
                {
                    return Unauthorized("ID do usuário não encontrado no token.");
                }

                await _userService.UpdatePasswordAsync(id, dto);
                return NoContent();
            }
            catch (DomainExceptionValidation ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

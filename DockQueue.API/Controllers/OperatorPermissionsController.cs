using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DockQueue.Application.DTOs.Permissions;
using DockQueue.Application.Interfaces;

namespace DockQueue.Api.Controllers
{
    [ApiController]
    [Route("api/operators/{userId:int}/permissions")]
    public class OperatorPermissionsController : ControllerBase
    {
        private readonly IOperatorPermissionsService _service;
        public OperatorPermissionsController(IOperatorPermissionsService service) { _service = service; }

        // Dashboard/Tela de Permissões consome isso para montar as 3 abas
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int userId)
        {
            var dto = await _service.GetByUserIdAsync(userId);
            if (dto is null) return NotFound();
            return Ok(dto);
        }

        // Atualiza as 3 abas de uma vez (status, boxes, telas)
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int userId, [FromBody] UpdateOperatorPermissionsDto dto)
        {
            var saved = await _service.UpsertAsync(userId, dto);
            return Ok(saved);
        }
    }
}

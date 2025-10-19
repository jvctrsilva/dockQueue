using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;

namespace DockQueue.Api.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly ISystemSettingsService _service;
        public SettingsController(ISystemSettingsService service) { _service = service; }

        // Qualquer autenticado pode visualizar
        [HttpGet("operating-schedule")]
        [Authorize]
        public async Task<IActionResult> GetOperatingSchedule()
        {
            var s = await _service.GetAsync();
            if (s is null) return NotFound();
            return Ok(s);
        }

        // Somente ADMIN pode atualizar
        [HttpPut("operating-schedule")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpsertOperatingSchedule([FromBody] UpdateSystemSettingsDto dto)
        {
            var saved = await _service.UpsertAsync(dto);
            return Ok(saved);
        }
    }
}

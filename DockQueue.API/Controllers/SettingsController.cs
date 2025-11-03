using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DockQueue.Api.Controllers
{
    [ApiController]
    [Route("api/settings")]
    [Authorize(Policy = "Screen:SettingsView")]
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
        [Authorize(Policy = "Screen:SettingsEdit")]
        public async Task<IActionResult> UpsertOperatingSchedule([FromBody] UpdateSystemSettingsDto dto)
        {
            var saved = await _service.UpsertAsync(dto); // usa o service
            return Ok(saved);
        }
    }
}

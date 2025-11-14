using DockQueue.Application.DTOs;
using DockQueue.Application.Services;
using DockQueue.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DockQueue.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "Screen:QueueView")]
    public class QueueController : ControllerBase
    {
        private readonly IQueueService _service;

        public QueueController(IQueueService service)
        {
            _service = service;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<QueueEntryViewDto>> Enqueue([FromBody] CreateQueueEntryDto dto)
        {
            try
            {
                int? userId = GetUserIdFromToken();
                var result = await _service.EnqueueAsync(dto, userId);
                return CreatedAtAction(nameof(GetQueue), new { type = result.Type }, result);
            }
            catch (InvalidOperationException ex)
            {
                // erro de negócio conhecido → retorna 400 com mensagem
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // erro inesperado → mantém o 500 padrão
                return StatusCode(500, "Erro interno ao processar o pedido.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<QueueEntryViewDto>>> GetQueue([FromQuery] QueueType type)
        {
            var result = await _service.GetQueueAsync(type);
            return Ok(result);
        }

        [HttpPut("status")]
        [Authorize(Policy = "Screen:QueueView")]
        public async Task<ActionResult<QueueEntryViewDto>> UpdateStatus([FromBody] UpdateQueueEntryStatusDto dto)
        {
            var userId = GetUserIdFromToken();
            var result = await _service.UpdateStatusAsync(dto, userId);
            return Ok(result);
        }

        [HttpPut("assign-box")]
        public async Task<ActionResult<QueueEntryViewDto>> AssignBox([FromBody] AssignBoxDto dto)
        {
            var userId = GetUserIdFromToken();
            var result = await _service.AssignBoxAsync(dto, userId);
            return Ok(result);
        }

        [HttpPost("start-box-operation")]
        public async Task<ActionResult<QueueEntryViewDto>> StartBoxOperation([FromBody] StartBoxOperationDto dto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _service.StartBoxOperationAsync(dto, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPost("finish-box-operation")]
        public async Task<ActionResult<QueueEntryViewDto>> FinishBoxOperation([FromBody] FinishBoxOperationDto dto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var result = await _service.FinishBoxOperationAsync(dto, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpDelete]
        private int? GetUserIdFromToken()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == "id");
            if (claim == null) return null;

            if (int.TryParse(claim.Value, out var id))
                return id;

            return null;
        }

        [HttpPost("lookup")]
        [AllowAnonymous]
        public async Task<ActionResult<QueueEntryViewDto>> Lookup([FromBody] DriverQueueLookupDto dto)
        {
            var result = await _service.GetDriverQueueEntryAsync(dto);
            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Clear([FromQuery] QueueType type)
        {
            await _service.ClearQueueAsync(type);
            return NoContent();
        }
    }
}

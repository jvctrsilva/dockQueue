using Microsoft.AspNetCore.Mvc;
using DockQueue.Application.Interfaces;
using DockQueue.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace DockQueue.API.Controllers
{
    [ApiController]
    [Authorize(Policy = "Screen:BoxesView")]
    [Route("api/[controller]")]
    public class BoxController : ControllerBase
    {
        private readonly IBoxService _boxService;

        public BoxController(IBoxService boxService)
        {
            _boxService = boxService;
        }

        // GET: api/box
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var boxes = await _boxService.GetAllBoxesAsync();
            return Ok(boxes);
        }

        // GET: api/box/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var box = await _boxService.GetBoxByIdAsync(id);
            if (box == null) return NotFound("Box não encontrado");
            return Ok(box);
        }

        // POST: api/box
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBoxDto createBoxDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var box = await _boxService.CreateBoxAsync(createBoxDto);
            return CreatedAtAction(nameof(GetById), new { id = box.Id }, box);
        }

        // PUT: api/box/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBoxDto updateBoxDto)
        {
            if (id != updateBoxDto.Id)
                return BadRequest("Id do box não confere");

            try
            {
                var updatedBox = await _boxService.UpdateBoxAsync(updateBoxDto);
                return Ok(updatedBox);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Box não encontrado");
            }
        }

        // DELETE: api/box/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _boxService.DeleteBoxAsync(id);
            return NoContent();
        }
    }
}

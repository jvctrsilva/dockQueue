using DockQueue.Application.DTOs;
using DockQueue.Application.Interfaces;
using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;

namespace DockQueue.Application.Services
{
    public class BoxService : IBoxService
    {
        private readonly IBoxRepository _boxRepository;

        public BoxService(IBoxRepository boxRepository)
        {
            _boxRepository = boxRepository;
        }

        public async Task<List<BoxDto>> GetAllBoxesAsync()
        {
            var boxes = await _boxRepository.GetAllAsync();

            return boxes.Select(b => new BoxDto
            {
                Id = b.Id,
                Name = b.Name,
                Status = b.Status,
                DriverId = b.DriverId,
                CreatedAt = b.CreatedAt
            }).ToList();
        }

        public async Task<BoxDto?> GetBoxByIdAsync(int id)
        {
            var box = await _boxRepository.GetByIdAsync(id);

            if (box == null) return null;

            return new BoxDto
            {
                Id = box.Id,
                Name = box.Name,
                Status = box.Status,
                DriverId = box.DriverId,
                CreatedAt = box.CreatedAt
            };
        }

        public async Task<BoxDto> CreateBoxAsync(CreateBoxDto createBoxDto)
        {
            var newBox = new Box(
                createBoxDto.Name,
                createBoxDto.Status,
                createBoxDto.DriverId,
                DateTime.Now
            );


            var createdBox = await _boxRepository.AddAsync(newBox);

            return new BoxDto
            {
                Id = createdBox.Id,
                Name = createdBox.Name,
                Status = createdBox.Status,
                DriverId = createdBox.DriverId,
                CreatedAt = createdBox.CreatedAt
            };
        }

        public async Task<BoxDto> UpdateBoxAsync(UpdateBoxDto updateBoxDto)
        {
            var box = await _boxRepository.GetByIdAsync(updateBoxDto.Id);
            if (box == null)
                throw new KeyNotFoundException("Box não encontrado");

            // atualizar campos da entidade
            box.Update(
                updateBoxDto.Name,
                updateBoxDto.Status,
                updateBoxDto.DriverId,
                box.CreatedAt
            );

            await _boxRepository.UpdateAsync(box);

            return new BoxDto
            {
                Id = box.Id,
                Name = box.Name,
                Status = box.Status,
                DriverId = box.DriverId,
                CreatedAt = box.CreatedAt
            };
        }

        public async Task DeleteBoxAsync(int id)
        {
            await _boxRepository.DeleteAsync(id);
        }
    }
}

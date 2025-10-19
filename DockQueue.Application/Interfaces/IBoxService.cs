
using DockQueue.Application.DTOs;

namespace DockQueue.Application.Interfaces
{
    public interface IBoxService
    {
        Task<List<BoxDto>> GetAllBoxesAsync();
        Task<BoxDto?> GetBoxByIdAsync(int id);
        Task<BoxDto> CreateBoxAsync(CreateBoxDto createBoxDto);
        Task<BoxDto> UpdateBoxAsync(UpdateBoxDto updateBoxDto);
        Task DeleteBoxAsync(int id);
    }
}

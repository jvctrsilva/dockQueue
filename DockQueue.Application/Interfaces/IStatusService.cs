using DockQueue.Application.DTOs;

namespace DockQueue.Application.Interfaces
{
    public interface IStatusService
    {
        Task<List<StatusDto>> GetAllAsync();
        Task<StatusDto> GetByIdAsync(int id);
        Task<StatusDto> CreateAsync(CreateStatusDto dto);
        Task<StatusDto> UpdateAsync(int id, UpdateStatusDto dto);

        Task DeleteAsync(int id);
    }
}

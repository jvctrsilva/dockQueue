using DockQueue.Domain.Entities;

namespace DockQueue.Domain.Interfaces
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByIdAsync(int id);

        Task<Driver?> FindByDocumentAsync(string documentNumber, string vehiclePlate);

        Task AddAsync(Driver driver);

        Task UpdateAsync(Driver driver);
    }
}

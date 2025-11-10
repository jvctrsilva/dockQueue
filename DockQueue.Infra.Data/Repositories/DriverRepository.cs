using DockQueue.Domain.Entities;
using DockQueue.Domain.Interfaces;
using DockQueue.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DockQueue.Infra.Data.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ApplicationDbContext _ctx;

        public DriverRepository(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Driver?> GetByIdAsync(int id)
        {
            return await _ctx.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Driver?> FindByDocumentAsync(
            string documentNumber,
            string vehiclePlate)
        {
            return await _ctx.Drivers
                .FirstOrDefaultAsync(d =>
                    d.DocumentNumber == documentNumber);
        }

        public async Task AddAsync(Driver driver)
        {
            _ctx.Drivers.Add(driver);
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Driver driver)
        {
            _ctx.Drivers.Update(driver);
            await _ctx.SaveChangesAsync();
        }
    }
}

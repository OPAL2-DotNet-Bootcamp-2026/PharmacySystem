using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class SupplyRepo
    {
        private readonly PharmacyContext context;

        public SupplyRepo(PharmacyContext context)
        {
            this.context = context;
        }

        // Returns all supply records with related data
        public async Task<List<Supply>> GetAllSupply()
        {
            return await context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.Medicine)
                .Include(s => s.Warehouse)
                .ToListAsync();
        }

        // Returns one supply using ID
        public async Task<Supply?> GetSupplyById(int id)
        {
            return await context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.Medicine)
                .Include(s => s.Warehouse)
                .FirstOrDefaultAsync(s => s.SupplyId == id);
        }

        // Adds a new supply
        public async Task Add(Supply supply)
        {
            await context.Supplies.AddAsync(supply);
            await context.SaveChangesAsync();
        }

        // Saves changes made to supply
        public async Task SupplyUpdate()
        {
            await context.SaveChangesAsync();
        }

        // Deletes supply
        public async Task SupplyDelete(Supply supply)
        {
            context.Supplies.Remove(supply);
            await context.SaveChangesAsync();
        }
    }
}
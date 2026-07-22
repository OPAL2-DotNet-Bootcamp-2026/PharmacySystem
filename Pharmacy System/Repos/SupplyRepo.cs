using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class SupplyRepo
    {
        private PharmacyContext context;

        public SupplyRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Returns all supply records with related data
        public async Task<List<Supply>> GetAllSupply()
        {
            return await context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.Medicine)
                .Include(s => s.Warehouse)
                .ToList();
        }

        // Returns one supply using  ID
        public async Task<Supply?> GetSupplyById(int id)
        {
            return await context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.Medicine)
                .Include(s => s.Warehouse)
                .FirstOrDefault(s => s.SupplyId == id);
        }

        // Adds  new supply 
        public async Task Add(Supply supply)
        {
            await context.Supplies.Add(supply);
            await context.SaveChanges();
        }

        // Save changes made to supply 
        public async Task SupplyUpdate()
        {
            await context.SaveChanges();
        }

        // Delete supply 
        public async Task SupplyDelete(Supply supply)
        {
            context.Supplies.Remove(supply);
           await context.SaveChanges();
        }
    }
}
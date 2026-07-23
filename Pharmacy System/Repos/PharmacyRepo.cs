using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacyRepo
    {
        private PharmacyContext context;

        public PharmacyRepo(PharmacyContext _context)
        {
            context = _context;
        }

        public async Task<List<Pharmacy>> GetAllPharmacy()
        {
            return await context.pharmacies.ToListAsync();
        }

        public async Task<Pharmacy?> GetPharmacyById(int id)
        {
            return await context.pharmacies.FirstOrDefaultAsync(p => p.PharmacyID == id);
        }

        public async Task<List<Pharmacy>> GetPharmacyByName(string name)
        {
            return await context.pharmacies.Where(n => n.PharmacyName == name) .ToListAsync();
        }

        public async Task<List<PharmacyStock>> GetPharmacyStockById(int id)
        {
            return await context.pharmacyStocks
                .Where(p => p.PharmacyID == id)
                .Include(p => p.medicine)
                .ToListAsync();
        }

        public async Task Add(Pharmacy pharmacy)
        {
            await context.pharmacies.AddAsync(pharmacy);
            await context.SaveChangesAsync();
        }

        public async Task Update()
        {
            await context.SaveChangesAsync();
        }

        public async Task Delete(Pharmacy pharmacy)
        {
            pharmacy.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
}

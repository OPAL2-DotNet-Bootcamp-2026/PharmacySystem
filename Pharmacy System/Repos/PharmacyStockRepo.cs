using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;

namespace Pharmacy_System.Repos
{
    public class PharmacyStockRepo
    {
        private PharmacyContext context;

        public PharmacyStockRepo(PharmacyContext _context)
        {
            context = _context;
        }

        public async Task<List<PharmacyStock>> GetPharmacyStockByPharmacyId(int id)
        {
            return await context.pharmacyStocks.Where(p => p.PharmacyID == id).ToListAsync();
        }

        public async Task<List<PharmacyStock>> GetPharmacyStockByMedicineId(int id)
        {
            return await context.pharmacyStocks.Where(p => p.MedicineID == id).ToListAsync();
        }

        public async Task<List<PharmacyStock>> GetPharmacyStockByLowStock(int id, int lowStock = 10)
        {
            return await context.pharmacyStocks.Where(p => p.PharmacyID == id && p.Quantity < lowStock).OrderBy(p => p.Quantity).ToListAsync();
        }
    }
}

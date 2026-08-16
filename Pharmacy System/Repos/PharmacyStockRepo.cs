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


        // Get all stock for one pharmacy
        public async Task<List<PharmacyStock>> GetPharmacyStockByPharmacyId(int id)
        {
            return await context.pharmacyStocks .Include(p => p.medicine).ThenInclude(m => m.Category).Where(p => p.PharmacyID == id).ToListAsync();
     
        }


        // Get stock records for one medicine
        public async Task<List<PharmacyStock>> GetPharmacyStockByMedicineId(int id)
        {
            return await context.pharmacyStocks.Include(p => p.medicine).ThenInclude(m => m.Category).Where(p => p.MedicineID == id).ToListAsync();
        }


        // Get one medicine stock from one pharmacy
        public async Task<PharmacyStock?> GetPharmacyStockAndMedicineById(int pid,int mid)
            
            
        {
            return await context.pharmacyStocks.Include(p => p.medicine).ThenInclude(m => m.Category) .FirstOrDefaultAsync(p => p.PharmacyID == pid && p.MedicineID == mid );     
        }


        // Get low stock medicines
        public async Task<List<PharmacyStock>> GetPharmacyStockByLowStock( int id, int lowStock = 10)
   
        {
            return await context.pharmacyStocks.Include(p => p.medicine).ThenInclude(m => m.Category).Where(p => p.PharmacyID == id && p.Quantity < lowStock).OrderBy(p => p.Quantity).ToListAsync();
       
                
        }


        // Add new pharmacy stock
        public async Task Add(PharmacyStock stock)
        {
            await context.pharmacyStocks.AddAsync(stock);

            await context.SaveChangesAsync();
        }


        // Update pharmacy stock
        public async Task Update()
        {
            await context.SaveChangesAsync();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class MedicineRepo
    {

        private PharmacyContext context;

        public MedicineRepo(PharmacyContext _context)   
        {
            context = _context;
        }

        public async Task<List<Medicine>> GetAllMedicine()
        {
            return await context.medicines
                .Where(m => m.IsActive)
                .ToListAsync();

        }

        public async Task<Medicine?> GetMedicineById(int id)   // Search for one medicine that has the same ID 
        {
            return await context.medicines.FirstOrDefaultAsync(m =>m.MedicineID == id &&m.IsActive);
        }

        public async Task Add(Medicine medicine)
        {
            await context.medicines.AddAsync(medicine);
            await context.SaveChangesAsync();
        }

        public async Task MedicineUpdate()
        {
            await context.SaveChangesAsync();
        }

        public async Task MedicineDelete(Medicine medicine)
        {
            medicine.IsActive = false;
            await context.SaveChangesAsync();
        }




    }
}

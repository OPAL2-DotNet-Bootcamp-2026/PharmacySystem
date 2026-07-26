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

        public async Task<List<Medicine>> GetAllMedicines()
        {
            return await context.medicines
                .Where(m => m.IsActive)
                .ToListAsync();

        }

        public async Task<Medicine?> GetMedicineById(int id)   // Search for one medicine that has the same ID 
        {
            return await context.medicines.FirstOrDefaultAsync(m =>m.MedicineID == id &&m.IsActive);
        }


      
        public async Task<List<Medicine>> GetAvailableMedicines()  // get available medicines
        {
            return await context.medicines.Where(m =>m.IsActive &&m.IsAvailable).ToListAsync();
        }


    
        // Get medicines by category
        public async Task<List<Medicine>> GetMedicinesByCategory(int categoryId)
        {
            return await context.medicines.Where(m =>m.IsActive &&m.CategoryID == categoryId).ToListAsync();
        }

        // Search medicines by name
        public async Task<List<Medicine>> SearchMedicinesByName(string name)
        {
            return await context.medicines.Where(m =>m.IsActive &&m.MedicineName.Contains(name))
                .ToListAsync();
        }

        public async Task AddNewMedicine(Medicine medicine)
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

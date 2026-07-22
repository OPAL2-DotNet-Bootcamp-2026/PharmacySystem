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

        public List<Medicine> GetAllMedicine()
        {
            return context.medicines.ToList();
        }

        public Medicine GetMedicineById(int id)   // Search for one medicine that has the same ID 
        {
            return context.medicines.FirstOrDefault(m => m.MedicineID == id);
        }

        public void Add(Medicine medicine)
        {
            context.medicines.Add(medicine);
            context.SaveChanges();
        }

        public void MedicineUpdate()
        {
            context.SaveChanges();
        }

        public void MedicineDelete(Medicine medicine)
        {
            medicine.IsActive = false;
            context.SaveChanges();
        }




    }
}

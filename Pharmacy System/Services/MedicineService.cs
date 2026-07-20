using Pharmacy_System.DTOs.Medicine;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class MedicineService
    {
        private MedicineRepo medicineRepo;
        private WarehouseRepo warehouseRepo;


        public MedicineService(MedicineRepo _medicineRepo, WarehouseRepo _warehouseRepo)  // Receive the repositories
        {
            medicineRepo = _medicineRepo;
            warehouseRepo = _warehouseRepo;
        }


        public List<MedicineDto> GetAllMedicines()       // Get all medicines and convert them to MedicineDto
        {
            List<Medicine> medicines = medicineRepo.GetAllMedicine();

            return medicines.Select(m => new MedicineDto
            {
                MedicineID = m.MedicineID,
                MedicineName = m.MedicineName,
                Category = m.Category,
                UnitPrice = m.UnitPrice,
                IsAvailable = m.isAvailable
            }).ToList();
        }

    }
}

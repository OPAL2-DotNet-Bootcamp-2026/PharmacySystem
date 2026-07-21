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
               
                UnitPrice = m.UnitPrice,
                IsAvailable = m.isAvailable
            }).ToList();
        }

      
        public MedicineDto? GetMedicineById(int id)   // get one medicine by ID
        {
            Medicine? medicine = medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return null;
            }

            return new MedicineDto
            {
                MedicineID = medicine.MedicineID,
                MedicineName = medicine.MedicineName,
                Category = medicine.Category,
                UnitPrice = medicine.UnitPrice,
                IsAvailable = medicine.isAvailable
            };
        }

      
        public int CreateMedicine(CreateMedicineDto dto)  // Create new medicine
        {
           
            Warehouse? warehouse = warehouseRepo.GetWarehouse();

            if (warehouse == null)
            {
                throw new Exception("Warehouse does not exist");  //404 not found
            }

            Medicine medicine = new Medicine
            {
                MedicineName = dto.MedicineName,
                Category = dto.Category,
                UnitPrice = dto.UnitPrice,

              
                isAvailable = true,

              
                WarehouseID = warehouse.WarehouseID  //   connect the medicine to the only warehouse
            };

            medicineRepo.Add(medicine);

            return medicine.MedicineID;
        }

       
        public bool UpdateMedicine(int id, UpdateMedicineDto dto)  //update
        {
            Medicine? medicine = medicineRepo.GetMedicineById(id); // ? optional

            if (medicine == null)
            {
                return false;
            }

            medicine.MedicineName = dto.MedicineName;
            medicine.Category = dto.Category;
            medicine.UnitPrice = dto.UnitPrice;
            medicine.isAvailable = dto.IsAvailable;

            medicineRepo.MedicineUpdate();

            return true;
        }

       
        public bool DeleteMedicine(int id)
        {
            Medicine? medicine = medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return false;
            }

            medicineRepo.MedicineDelete(medicine);

            return true;
        }

    }
}

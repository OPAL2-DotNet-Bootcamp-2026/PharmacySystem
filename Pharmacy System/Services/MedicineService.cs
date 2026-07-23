using Microsoft.EntityFrameworkCore;
using Pharmacy_System.DTOs.Medicine;
using Pharmacy_System.DTOs.WarehouseStock;
using Pharmacy_System.Models;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class MedicineService
    {

        private readonly MedicineRepo medicineRepo;

        public MedicineService(MedicineRepo _medicineRepo)
        {
            medicineRepo = _medicineRepo;
        }


        // Get all active medicines
        public async Task<List<MedicineDto>> GetAll()
        {
            List<Medicine> medicines = await medicineRepo.GetAllMedicines();

            return medicines.Select(m => new MedicineDto
            {
                MedicineID = m.MedicineID,
                MedicineName = m.MedicineName,
                CategoryID = m.CategoryID,
                UnitPrice = m.UnitPrice,
                IsAvailable = m.IsAvailable
            }).ToList();
        }


        // Get one medicine by ID
        public async Task<MedicineDto?> GetById(int id)
        {
            Medicine? medicine = await medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return null;
            }

            return new MedicineDto
            {
                MedicineID = medicine.MedicineID,
                MedicineName = medicine.MedicineName,
                CategoryID = medicine.CategoryID,
                UnitPrice = medicine.UnitPrice,
                IsAvailable = medicine.IsAvailable
            };
        }


        // Get available medicines
        public async Task<List<MedicineDto>> GetAvailable()
        {
            List<Medicine> medicines = await medicineRepo.GetAvailableMedicines();

            return medicines.Select(m => new MedicineDto
            {
                MedicineID = m.MedicineID,
                MedicineName = m.MedicineName,
                CategoryID = m.CategoryID,
                UnitPrice = m.UnitPrice,
                IsAvailable = m.IsAvailable
            }).ToList();
        }


        // Get medicines by category
        public async Task<List<MedicineDto>> GetByCategory(int categoryId)
        {
            List<Medicine> medicines = await medicineRepo.GetMedicinesByCategory(categoryId);

            return medicines.Select(m => new MedicineDto
            {
                MedicineID = m.MedicineID,
                MedicineName = m.MedicineName,
                CategoryID = m.CategoryID,
                UnitPrice = m.UnitPrice,
                IsAvailable = m.IsAvailable
            }).ToList();
        }


        // Search medicines by name
        public async Task<List<MedicineDto>> SearchByName(string name)
        {
            List<Medicine> medicines = await medicineRepo.SearchMedicinesByName(name);

            return medicines.Select(m => new MedicineDto
            {
                MedicineID = m.MedicineID,
                MedicineName = m.MedicineName,
                CategoryID = m.CategoryID,
                UnitPrice = m.UnitPrice,
                IsAvailable = m.IsAvailable
            }).ToList();
        }


        // Change medicine availability
        public async Task<bool> ToggleAvailability(int id)
        {
            Medicine? medicine = await medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return false;
            }

            // True becomes false, and false becomes true
            medicine.IsAvailable = !medicine.IsAvailable;

            await medicineRepo.MedicineUpdate();

            return true;
        }



        // Add a new medicine
        public async Task<int> AddNewMedicine(CreateMedicineDto dto)
        {
            Medicine medicine = new Medicine
            {
                MedicineName = dto.MedicineName,
                CategoryID = dto.CategoryID,
                UnitPrice = dto.UnitPrice,
                IsAvailable = true,
                IsActive = true
            };

            await medicineRepo.AddNewMedicine(medicine);

            return medicine.MedicineID;
        }

        // Update medicine information
        public async Task<bool> Update(int id, UpdateMedicineDto dto)
        {
            Medicine? medicine = await medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return false;
            }

            medicine.MedicineName = dto.MedicineName;
            medicine.CategoryID = dto.CategoryID;
            medicine.UnitPrice = dto.UnitPrice;

            await medicineRepo.MedicineUpdate();

            return true;
        }

        //// Soft delete medicine
        //public async Task<bool> Delete(int id)
        //{
        //    Medicine? medicine =
        //        await medicineRepo.GetMedicineById(id);

        //    if (medicine == null)
        //    {
        //        return false;
        //    }

        //    await medicineRepo.MedicineDelete(medicine);

        //    return true;
        //}


    }
}

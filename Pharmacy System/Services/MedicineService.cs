using Microsoft.EntityFrameworkCore;
using Pharmacy_System.DTOs.Medicine;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class MedicineService
    {

            private readonly MedicineRepo MedicineRepo;
            private readonly CategoryRepo CategoryRepo;
            private readonly SupplyRepo SupplyRepo;
            private readonly WarehouseStockRepo WarehouseStockRepo;


            public MedicineService(
                MedicineRepo _medicineRepo,
                CategoryRepo _categoryRepo,
                SupplyRepo _supplyRepo,
                WarehouseStockRepo _warehouseStockRepo)
            {
                MedicineRepo = _medicineRepo;
                CategoryRepo = _categoryRepo;
                SupplyRepo = _supplyRepo;
                WarehouseStockRepo = _warehouseStockRepo;
            }



        public async Task<List<MedicineDto>> GetAllMedicines()       // Get all medicines and convert them to MedicineDto
        {
            List<Medicine> medicines =await MedicineRepo.GetAllMedicine();

            return medicines.Select(m => new MedicineDto
            {
                MedicineID = m.MedicineID,
                MedicineName = m.MedicineName,
                CategoryID = m.CategoryID,
                UnitPrice = m.UnitPrice,
                IsAvailable = m.IsAvailable,
                IsActive = m.IsActive,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            }).ToList();
        }

      
        public async Task<MedicineDto?> GetMedicineById(int id)   // get one medicine by ID
        {
            Medicine? medicine =await MedicineRepo.GetMedicineById(id);

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
                IsAvailable = medicine.IsAvailable,
                IsActive = medicine.IsActive,
                CreatedAt = medicine.CreatedAt,
                UpdatedAt = medicine.UpdatedAt
            };
        }


        //public async Task<int> CreateMedicine(CreateMedicineDto dto)
        //{
        //    Category? category = await CategoryRepo.GetCategoryById(dto.CategoryID);

        //    if (category == null)
        //    {
        //        throw new Exception("Category does not exist");
        //    }

        //    Medicine medicine = new Medicine
        //    {
        //        MedicineName = dto.MedicineName,
        //        CategoryID = dto.CategoryID,
        //        UnitPrice = dto.UnitPrice,
        //        isAvailable = true,
        //        IsActive = true
        //    };

        //    await MedicineRepo.Add(medicine);

        //    return medicine.MedicineID;
        //}


        public async Task<bool> UpdateMedicine(int id, UpdateMedicineDto dto)  //update
        {
            Medicine? medicine = await MedicineRepo.GetMedicineById(id); 

            if (medicine == null)
            {
                return false;
            }


            //// Check that the category exists
            //Category? category = await CategoryRepo.GetCategoryById(dto.CategoryID);

            //if (category == null)
            //{
            //    throw new Exception("Category does not exist");
            //}


            medicine.MedicineName = dto.MedicineName;
            medicine.CategoryID = dto.CategoryID;
            medicine.UnitPrice = dto.UnitPrice;
            medicine.IsAvailable = dto.IsAvailable;

            await MedicineRepo.MedicineUpdate();

            return true;
        }

        //public async Task<bool> DeleteMedicine(int id)
        //{
        //    Medicine? medicine = await MedicineRepo.GetMedicineById(id);

        //    if (medicine == null)
        //    {
        //        return false;
        //    }

        //    await MedicineRepo.MedicineDelete(medicine);

        //    return true;
        //}

    }
}

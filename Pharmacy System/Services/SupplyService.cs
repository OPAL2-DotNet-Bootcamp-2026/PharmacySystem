using Pharmacy_System.DTOs.Supply;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class SupplyService
    {
        private readonly SupplyRepo supplyRepo;
        private readonly SupplierRepo supplierRepo;
        private readonly MedicineRepo medicineRepo;
        private readonly WarehouseRepo warehouseRepo;

        public SupplyService(
            SupplyRepo supplyRepo,
            SupplierRepo supplierRepo,
            MedicineRepo medicineRepo,
            WarehouseRepo warehouseRepo)
        {
            this.supplyRepo = supplyRepo;
            this.supplierRepo = supplierRepo;
            this.medicineRepo = medicineRepo;
            this.warehouseRepo = warehouseRepo;
        }

        // Returns all supplies as DTOs
        public async Task<List<SupplyDto>> GetAllSupplies()
        {
            List<Supply> supplies =
                await supplyRepo.GetAllSupply();

            List<SupplyDto> supplyDtos =
                new List<SupplyDto>();

            foreach (Supply supply in supplies)
            {
                SupplyDto dto = ConvertToDto(supply);
                supplyDtos.Add(dto);
            }

            return supplyDtos;
        }

        // Returns one supply using its ID
        public async Task<SupplyDto?> GetSupplyById(int id)
        {
            Supply? supply =
                await supplyRepo.GetSupplyById(id);

            if (supply == null)
            {
                return null;
            }

            return ConvertToDto(supply);
        }

        // Creates a new supply and returns its generated ID
        public async Task<int> CreateSupply(CreateSupplyDto dto)
        {
            Supplier? supplier =await supplierRepo.GetSupplierById(dto.SupplierID);
    

            if (supplier == null)
            {
                throw new Exception("Supplier not found");
            }

            Medicine? medicine = await medicineRepo.GetMedicineById(dto.MedicineID);
    


            if (medicine == null)
            {
                throw new Exception("Medicine not found");
            }

            Warehouse? warehouse =await warehouseRepo.GetWarehouseById(dto.WarehouseID);
     

            if (warehouse == null)
            {
                throw new Exception("Warehouse not found");
            }

            if (dto.ExpiryDate == null)
            {
                throw new Exception("Expiry date is required");
            }

            if (dto.ExpiryDate.Value.Date <= DateTime.Today)
            {
                throw new Exception(
                    "Expiry date must be later than today"
                );
            }

            Supply supply = new Supply();

            supply.BatchNumber = dto.BatchNumber;
            supply.Quantity = dto.Quantity;
            supply.UnitCost = dto.UnitCost;

            // Total cost = quantity × unit cost
            supply.TotalCost =
                dto.Quantity * dto.UnitCost;

            supply.SupplyDate = DateTime.Now;
            supply.ExpiryDate = dto.ExpiryDate.Value;

            supply.SupplierID = dto.SupplierID;
            supply.MedicineID = dto.MedicineID;
            supply.WarehouseID = dto.WarehouseID;

            await supplyRepo.Add(supply);

            return supply.SupplyId;
        }

        // Updates an existing supply
        public async Task<bool> UpdateSupply(
            int id,
            UpdateSupplyDto dto)
        {
            Supply? supply =await supplyRepo.GetSupplyById(id);
                

            if (supply == null)
            {
                return false;
            }

            Supplier? supplier =await supplierRepo.GetSupplierById(dto.SupplierID);
      

            if (supplier == null)
            {
                throw new Exception("Supplier not found");
            }

            Medicine? medicine = await medicineRepo.GetMedicineById(dto.MedicineID);
   

            if (medicine == null)
            {
                throw new Exception("Medicine not found");
            }

            Warehouse? warehouse =await warehouseRepo.GetWarehouseById(dto.WarehouseID);
    

            if (warehouse == null)
            {
                throw new Exception("Warehouse not found");
            }

            if (dto.ExpiryDate == null)
            {
                throw new Exception("Expiry date is required");
            }

            if (dto.ExpiryDate.Value.Date <= DateTime.Today)
            {
                throw new Exception(
                    "Expiry date must be later than today"
                );
            }

            supply.BatchNumber = dto.BatchNumber;
            supply.Quantity = dto.Quantity;
            supply.UnitCost = dto.UnitCost;

            supply.TotalCost =
                dto.Quantity * dto.UnitCost;

            supply.ExpiryDate = dto.ExpiryDate.Value;
            supply.SupplierID = dto.SupplierID;
            supply.MedicineID = dto.MedicineID;
            supply.WarehouseID = dto.WarehouseID;

            await supplyRepo.SupplyUpdate();

            return true;
        }

        // Deletes an existing supply
        public async Task<bool> DeleteSupply(int id)
        {
            Supply? supply =
                await supplyRepo.GetSupplyById(id);

            if (supply == null)
            {
                return false;
            }

            await supplyRepo.SupplyDelete(supply);

            return true;
        }

        // Converts Supply model into SupplyDto
        private SupplyDto ConvertToDto(Supply supply)
        {
            SupplyDto dto = new SupplyDto();

            dto.SupplyId = supply.SupplyId;
            dto.BatchNumber = supply.BatchNumber;
            dto.Quantity = supply.Quantity;
            dto.UnitCost = supply.UnitCost;
            dto.TotalCost = supply.TotalCost;
            dto.SupplyDate = supply.SupplyDate;
            dto.ExpiryDate = supply.ExpiryDate;

            dto.SupplierID = supply.SupplierID;
            dto.FullName = supply.Supplier.FullName;

            dto.MedicineID = supply.MedicineID;
            dto.MedicineName =
                supply.Medicine.MedicineName;

            dto.WarehouseID = supply.WarehouseID;
            dto.Location = supply.Warehouse.Location;

            return dto;
        }
    }
}
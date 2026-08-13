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

        private readonly WarehouseStockService warehouseStockService;

        private readonly PharmacyContext context;

        public SupplyService(
            SupplyRepo supplyRepo,
            SupplierRepo supplierRepo,
            MedicineRepo medicineRepo,
            WarehouseRepo warehouseRepo,
            WarehouseStockService warehouseStockService,
            PharmacyContext context)
        {
            this.supplyRepo = supplyRepo;
            this.supplierRepo = supplierRepo;
            this.medicineRepo = medicineRepo;
            this.warehouseRepo = warehouseRepo;
            this.warehouseStockService = warehouseStockService;
            this.context = context;
        }

       
        public async Task<List<SupplyDto>> GetAllSupplies()
        {

            List<Supply> supplies = await supplyRepo.GetAllSupply();


            List<SupplyDto> supplyDtos = new List<SupplyDto>();


            foreach (Supply supply in supplies)
            {
                SupplyDto dto = ConvertToDto(supply);
                supplyDtos.Add(dto);
            }

            return supplyDtos;
        }

        // Gets one supply using its ID
        public async Task<SupplyDto?> GetSupplyById(int id)
        {
            Supply? supply = await supplyRepo.GetSupplyById(id);


            if (supply == null)
            {
                return null;
            }

            return ConvertToDto(supply);
        }

        // Creates a new supply
        public async Task<int> CreateSupply(CreateSupplyDto dto)
        {
            // Check that the supplier exists
            Supplier? supplier = await supplierRepo.GetSupplierById(dto.SupplierID);


            if (supplier == null)
            {
                throw new Exception("Supplier not found");
            }

            // Check that the medicine exists
            Medicine? medicine = await medicineRepo.GetMedicineById(dto.MedicineID);


            if (medicine == null)
            {
                throw new Exception("Medicine not found");
            }

            // Check that the warehouse exists
            Warehouse? warehouse = await warehouseRepo.GetWarehouseById(dto.WarehouseID);


            if (warehouse == null)
            {
                throw new Exception("Warehouse not found");
            }

            // Check that the expiry date was provided
            if (dto.ExpiryDate == null)
            {
                throw new Exception("Expiry date is required");
            }

            // Prevent adding expired medicine
            if (dto.ExpiryDate.Value.Date <= DateTime.Today)
            {
                throw new Exception("Expiry date must be later than today");
            }

            using var tx = await context.Database.BeginTransactionAsync();

            Supply supply = new Supply();

            supply.BatchNumber = dto.BatchNumber;
            supply.Quantity = dto.Quantity;
            supply.UnitCost = dto.UnitCost;

            supply.TotalCost = dto.Quantity * dto.UnitCost;
            supply.SupplyDate = DateTime.Now;

            supply.ExpiryDate = dto.ExpiryDate.Value;

            //  foreign keys
            supply.SupplierID = dto.SupplierID;
            supply.MedicineID = dto.MedicineID;
            supply.WarehouseID = dto.WarehouseID;

            

            //to repo then add in database
            await supplyRepo.Add(supply);

            await warehouseStockService.Increase(
                supply.WarehouseID,
                supply.MedicineID,
                supply.Quantity,
                DateOnly.FromDateTime(supply.ExpiryDate));

            await tx.CommitAsync();

            return supply.SupplyId;
        }

        //update

        public async Task<bool> UpdateSupply(int id, UpdateSupplyDto dto)
        {
            Supply? supply = await supplyRepo.GetSupplyById(id);

            if (supply == null)
            {
                return false;
            }

            // Check supplier
            Supplier? supplier =
                await supplierRepo.GetSupplierById(dto.SupplierID);

            if (supplier == null)
            {
                throw new Exception("Supplier not found");
            }

            // Check medicine
            Medicine? medicine =
                await medicineRepo.GetMedicineById(dto.MedicineID);

            if (medicine == null)
            {
                throw new Exception("Medicine not found");
            }

            // Check warehouse
            Warehouse? warehouse =
                await warehouseRepo.GetWarehouseById(dto.WarehouseID);

            if (warehouse == null)
            {
                throw new Exception("Warehouse not found");
            }

            // Check expiry date
            if (dto.ExpiryDate == null)
            {
                throw new Exception("Expiry date is required");
            }

            if (dto.ExpiryDate.Value.Date <= DateTime.Today)
            {
                throw new Exception("Expiry date must be later than today");
            }

            // Save OLD values before updating
            int oldWarehouseId = supply.WarehouseID;
            int oldMedicineId = supply.MedicineID;
            int oldQuantity = supply.Quantity;

            using var tx = await context.Database.BeginTransactionAsync();

            try
            {
                /*
                 * CASE 1:
                 * Same warehouse and same medicine.
                 * We only update the quantity difference.
                 */
                if (oldWarehouseId == dto.WarehouseID &&
                    oldMedicineId == dto.MedicineID)
                {
                    int difference = dto.Quantity - oldQuantity;

                    // Quantity increased
                    if (difference > 0)
                    {
                        await warehouseStockService.Increase(
                            dto.WarehouseID,
                            dto.MedicineID,
                            difference,
                            DateOnly.FromDateTime(dto.ExpiryDate.Value)
                        );
                    }

                    // Quantity decreased
                    else if (difference < 0)
                    {
                        await warehouseStockService.Decrease(
                            oldWarehouseId,
                            oldMedicineId,
                            Math.Abs(difference)
                        );
                    }
                }

                /*
                 * CASE 2:
                 * Warehouse or Medicine changed.
                 *
                 * Remove quantity from old stock,
                 * then add it to the new stock.
                 */
                else
                {
                    await warehouseStockService.Decrease(
                        oldWarehouseId,
                        oldMedicineId,
                        oldQuantity
                    );

                    await warehouseStockService.Increase(
                        dto.WarehouseID,
                        dto.MedicineID,
                        dto.Quantity,
                        DateOnly.FromDateTime(dto.ExpiryDate.Value)
                    );
                }

                // Update Supply information
                supply.BatchNumber = dto.BatchNumber;
                supply.Quantity = dto.Quantity;
                supply.UnitCost = dto.UnitCost;
                supply.TotalCost = dto.Quantity * dto.UnitCost;

                supply.ExpiryDate = dto.ExpiryDate.Value;
                supply.SupplierID = dto.SupplierID;
                supply.MedicineID = dto.MedicineID;
                supply.WarehouseID = dto.WarehouseID;

                await supplyRepo.SupplyUpdate();

                await tx.CommitAsync();

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        //delete

        public async Task<bool> DeleteSupply(int id)
        {
            Supply? supply = await supplyRepo.GetSupplyById(id);

            if (supply == null)
            {
                return false;
            }

            using var tx = await context.Database.BeginTransactionAsync();

            try
            {
                // Remove supply quantity from warehouse stock
                await warehouseStockService.Decrease(
                    supply.WarehouseID,
                    supply.MedicineID,
                    supply.Quantity
                );

                // Delete / deactivate supply
                await supplyRepo.SupplyDelete(supply);

                await tx.CommitAsync();

                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
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
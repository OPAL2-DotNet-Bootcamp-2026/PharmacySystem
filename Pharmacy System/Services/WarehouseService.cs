using Pharmacy_System.DTOs.Warehouse;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;

namespace Pharmacy_System.Services
{
    public class WarehouseService
    {
        private WarehouseRepo warehouseRepo;

    
        public WarehouseService(WarehouseRepo _warehouseRepo)     //receive the warehouse repository
        {
            warehouseRepo = _warehouseRepo;
        }

       
        public WarehouseDto? GetWarehouse() //get the warehouse information
        {
            Warehouse? warehouse = warehouseRepo.GetWarehouse();

           
            if (warehouse == null) // Return null if the warehouse does not exist
            {
                return null;
            }

            
            return new WarehouseDto  //Convert Warehouse model to WarehouseDto
            {
                WarehouseID = warehouse.WarehouseID,
                Location = warehouse.Location,
                Quantity = warehouse.Quantity,
                ExpiryDate = warehouse.ExpiryDate
            };
        }

    
        public int CreateWarehouse(CreateWarehouseDto dto) // create
        {
           
            if (warehouseRepo.WarehouseExists()) //system have only one warehouse so we need to check cannot add warehouse if already exsists.
            {
                throw new Exception("The warehouse already exists");
            }

           
            Warehouse warehouse = new Warehouse // // Convert CreateWarehouseDto to Warehouse model
            {
                Location = dto.Location,
                Quantity = dto.Quantity,
                ExpiryDate = dto.ExpiryDate
            };

            // Save the warehouse in the database
            warehouseRepo.Add(warehouse);

            return warehouse.WarehouseID;  // return auto generated warehouseID
        }

       
        public bool UpdateWarehouse(UpdateWarehouseDto dto) // update
        {
            Warehouse? warehouse = warehouseRepo.GetWarehouse();

           
            if (warehouse == null)  // so it stop if the warehouse does not exist
            {
                return false;
            }

           //upate the information:
            warehouse.Location = dto.Location;
            warehouse.Quantity = dto.Quantity;
            warehouse.ExpiryDate = dto.ExpiryDate;

            
            warehouseRepo.WarehouseUpdate(); // save changes

            return true;
        }

        //delete warehouse => do not needed because our system has one permanent warehouse.
    }
}

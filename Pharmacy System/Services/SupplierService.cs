using Pharmacy_System.DTOs.Supplier;
using Pharmacy_System.Modules;
using Pharmacy_System.Repos;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Pharmacy_System.Services
{
    public class SupplierService
    {

        private SupplierRepo supplierRepo;


        public SupplierService(SupplierRepo _supplierRepo) //receive the supplier repository
        {
            supplierRepo = _supplierRepo;
        }


        public async Task<List<SupplierDto>> GetAllSuppliers() //get all suppliers
        {
            List<Supplier> suppliers = await supplierRepo.GetAllSuppliers();

            // Convert Supplier models into SupplierDto objects
            return suppliers.Select(s => new SupplierDto
            {
                SupplierID = s.SupplierID,
                FullName = s.FullName,
                Phone = s.Phone,
                Email = s.Email,
                Location = s.Location
            }).ToList();
        }


        public async Task<SupplierDto?> GetSupplierById(int id) //  get one supplier by ID
        {
            Supplier? supplier = await supplierRepo.GetSupplierById(id);


            // Return null if the supplier does not exist
            if (supplier == null)
            {
                return null;
            }


            return new SupplierDto  //Convert Supplier model into SupplierDto
            {
                SupplierID = supplier.SupplierID,
                FullName = supplier.FullName,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Location = supplier.Location
            };
        }


        public async Task<bool> CreateSupplier(CreateSupplierDto dto)
        {

            bool emailExists = await supplierRepo.EmailExists(dto.Email);
            // Stop if the email already exists
            if (emailExists)
            {
                return false;
            }

            Supplier supplier = new Supplier
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Location = dto.Location
            };

            supplierRepo.Add(supplier);

            return true;
        }


        public async Task<bool> UpdateSupplier(int id,UpdateSupplierDto dto)
        {
            Supplier? supplier = await supplierRepo.GetSupplierById(id);


            if (supplier == null)
            {
                return false;
            }
            // Find whether another supplier uses this email
            Supplier? supplierWithEmail = await supplierRepo.GetSupplierByEmail(dto.Email);

            // Check whether another supplier already uses this email
            if (supplierWithEmail != null && supplierWithEmail.SupplierID != id)
            {
                throw new Exception("Supplier email already exists");
            }

            // Change supplier information
            supplier.FullName = dto.FullName;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Location = dto.Location;

          
            supplierRepo.SupplierUpdate();

            return true;
        }

    //    // Delete a supplier
    //    public async Task<bool> DeleteSupplier(int id)
    //    {
    //        Supplier? supplier = await supplierRepo.GetSupplierById(id);

    //        // Stop if the supplier does not exist
    //        if (supplier == null)
    //        {
    //            return false;
    //        }

    //        supplierRepo.SupplierDelete(supplier);

    //        return true;
    //    }
    //}


}
 


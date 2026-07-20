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

        public List<SupplierDto> GetAllSuppliers() //get all suppliers
        {

            List<Supplier> suppliers = supplierRepo.GetAllSuppliers();

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


        public SupplierDto? GetSupplierById(int id) //  get one supplier by ID
        {
            Supplier? supplier = supplierRepo.GetSupplierById(id);

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


        public bool CreateSupplier(CreateSupplierDto dto)
        {
            // Stop if the email already exists
            if (supplierRepo.EmailExists(dto.Email))
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

    
    public bool UpdateSupplier(int id, UpdateSupplierDto dto) //update
        {
            Supplier? supplier = supplierRepo.GetSupplierById(id);

            // Stop if the supplier does not exist
            if (supplier == null)
            {
                return false;
            }

            // Find whether another supplier uses this email
            Supplier? supplierWithEmail =supplierRepo.GetSupplierByEmail(dto.Email);

         
            if (supplierWithEmail != null &&supplierWithEmail.SupplierID != id) // email must be unique 
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

        // Delete a supplier
        public bool DeleteSupplier(int id)
        {
            Supplier? supplier = supplierRepo.GetSupplierById(id);

            // Stop if the supplier does not exist
            if (supplier == null)
            {
                return false;
            }

            supplierRepo.SupplierDelete(supplier);

            return true;
        }
    }


}
 


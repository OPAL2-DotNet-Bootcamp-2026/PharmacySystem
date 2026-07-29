using Pharmacy_System.DTOs.Supplier;
using Pharmacy_System.DTOs.User;
using Pharmacy_System.Models;
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


        public async Task<List<SupplierDto>> GetAllSuppliers() 
        {
            List<Supplier> suppliers = await supplierRepo.GetAllSuppliers();

           
            return suppliers.Select(s => new SupplierDto
            {
                SupplierID = s.SupplierID,
                FullName = s.FullName,
                Phone = s.Phone,
                Email = s.Email,
                Location = s.Location,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();
        }


        public async Task<SupplierDto?> GetSupplierById(int id) 
        {
            Supplier? supplier = await supplierRepo.GetSupplierById(id);


           
            if (supplier == null)
            {
                return null;
            }


            return new SupplierDto() {

                SupplierID = supplier.SupplierID,
                FullName = supplier.FullName,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Location = supplier.Location,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                UpdatedAt = supplier.UpdatedAt
            };

        }

        public async Task<List<SupplierDto>> GetByLocation(string location)
        {
            List<Supplier> suppliers = await supplierRepo.GetByLocation(location);

            return suppliers.Select(s => new SupplierDto
            {
                SupplierID = s.SupplierID,
                FullName = s.FullName,
                Phone = s.Phone,
                Email = s.Email,
                Location = s.Location,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();
        }


        public async Task<int> CreateSupplier(CreateSupplierDto dto)
        {
            bool emailExists = await supplierRepo.EmailExists(dto.Email);

            if (emailExists)
            {
                throw new Exception("Supplier email already exists");
            }


            Supplier supplier = new Supplier()
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Location = dto.Location,
                IsActive = true
            };

         
            await supplierRepo.Add(supplier);

            return supplier.SupplierID;
        }



        public async Task<bool> UpdateSupplier(int id,UpdateSupplierDto dto)
        {
            Supplier? supplier = await supplierRepo.GetSupplierById(id);


            if (supplier == null)
            {
                return false;
            }
           
            Supplier? supplierWithEmail = await supplierRepo.GetSupplierByEmail(dto.Email);

           
            if (supplierWithEmail != null && supplierWithEmail.SupplierID != id)
            {
                throw new Exception("Supplier email already exists");
            }

            // Change supplier info
            supplier.FullName = dto.FullName;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Location = dto.Location;

            
            await supplierRepo.SupplierUpdate();

            return true;
        }

        public async Task<bool> DeleteSupplier(int id)
        {
            Supplier? supplier = await supplierRepo.GetSupplierById(id);

            if (supplier == null)
            {
                return false;
            }

            await supplierRepo.SupplierDelete(supplier);

            return true;
        }
    }


}
 


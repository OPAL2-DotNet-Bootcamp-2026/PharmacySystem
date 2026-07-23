using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class SupplierRepo
    {
        private PharmacyContext context;

        public SupplierRepo(PharmacyContext _context)
        {
            context = _context;
        }

        public async Task<List<Supplier>> GetAllSuppliers()  // get all suppliers from the database
        {
            return await context.suppliers.Where(s => s.IsActive).ToListAsync();
        }

        public async Task<Supplier?> GetSupplierById(int id)
        {
            return await context.suppliers.FirstOrDefaultAsync(s =>s.SupplierID == id &&s.IsActive);
        }

        
        public async Task<Supplier?> GetSupplierByEmail(string email) // find a supplier by email
        {
            return await context.suppliers.FirstOrDefaultAsync(s => s.Email == email &&s.IsActive);
        }

        public async Task<bool> EmailExists(string email)  // to check if the email exists
        {
            return await context.suppliers.AnyAsync(s => s.Email == email);
        }

        public async Task Add(Supplier supplier)  // add a new supplier to the database
        {
            await context.suppliers.AddAsync(supplier);
            await context.SaveChangesAsync();
        }


        public async Task SupplierUpdate() // save changes made to a supplier
        {
            await context.SaveChangesAsync();
        }

        
        public void SupplierDelete(Supplier supplier)  //  Delete a supplier from the database
        {
            context.suppliers.Remove(supplier);
            context.SaveChanges();
        }



    }
}

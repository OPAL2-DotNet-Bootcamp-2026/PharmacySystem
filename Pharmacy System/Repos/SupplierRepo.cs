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

        public List<Supplier> GetAllSuppliers()  // get all suppliers from the database
        {
            return context.suppliers.ToList();
        }

        public Supplier? GetSupplierById(int id)
        {
            return context.suppliers.FirstOrDefault(s => s.SupplierID == id);
        }

        
        public Supplier? GetSupplierByEmail(string email) // find a supplier by email
        {
            return context.suppliers.FirstOrDefault(s => s.Email == email);
        }

        public bool EmailExists(string email)  // to check if the email exists
        {
            return context.pharmacists.Any(e => e.Email == email);
        }

        public void Add(Supplier supplier)  // add a new supplier to the database
        {
            context.suppliers.Add(supplier);
            context.SaveChanges();
        }

        
        public void SupplierUpdate() // save changes made to a supplier
        {
            context.SaveChanges();
        }

        
        public void SupplierDelete(Supplier supplier)  //  Delete a supplier from the database
        {
            context.suppliers.Remove(supplier);
            context.SaveChanges();
        }



    }
}

using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class CustomerRepo
    {
        private PharmacyContext context;

        public CustomerRepo(PharmacyContext _context)
        {
            context = _context;
        }

        public List<Customer> GetAllCustomer()
        {
            return context.customers.ToList();
        }

        public Customer GetCustomerById(int id)
        {
            return context.customers.FirstOrDefault(p => p.CustomerID == id);
        }

        public void Add(Customer customers)
        {
            context.customers.Add(customers);
            context.SaveChanges();
        }

        public void CustomerUpdate()
        {
            context.SaveChanges();
        }

        public void CustomerDelete(Customer customers)
        {
            context.customers.Remove(customers);
            context.SaveChanges();
        }
    }
}

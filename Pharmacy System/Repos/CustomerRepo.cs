using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Customer>> GetAllCustomer()
        {
            return await context.customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerById(int id)
        {
            return await context.customers.FirstOrDefaultAsync(p => p.CustomerID == id);
        }

        public async Task<List<Customer>> GetCustomerByName(string name)
        {
            return await context.customers.Where(c => c.FullName.Contains(name)).ToListAsync();
        }

        public async Task<Customer?> GetCustomerByPhone(string phone)
        {
            return await context.customers.FirstOrDefaultAsync(p => p.Phone == phone);
        }

        public async Task<List<CustomerOrder>> GetCustomerOrdersById(int id)
        {
            return await context.customerOrders.Where(o => o.CustomerId == id).ToListAsync();
        }

        public async Task Add(Customer customers)
        {
            await context.customers.AddAsync(customers);
            await context.SaveChangesAsync();
        }

        public async Task CustomerUpdate()
        {
            await context.SaveChangesAsync();
        }

        public async Task CustomerDelete(Customer customers)
        {
            customers.IsActive = false;
            await context.SaveChangesAsync();
        }
    }
}

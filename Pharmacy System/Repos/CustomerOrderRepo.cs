using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class CustomerOrderRepo
    {
        private readonly PharmacyContext context;

        public CustomerOrderRepo(PharmacyContext context)
        {
            this.context = context;
        }

        // Returns all customer orders with related data
        public async Task<List<CustomerOrder>> GetAllCustomerOrders()
        {
            return await context.customerOrders
                .Include(o => o.Customer)
                .Include(o => o.Pharmacy)
                .Include(o => o.CustomerOrderDetails)
                    .ThenInclude(d => d.Medicine)
                .ToListAsync();
        }

        // Returns one customer order using its ID
        public async Task<CustomerOrder?> GetCustomerOrderById(int id)
        {
            return await context.customerOrders
                .Include(o => o.Customer)
                .Include(o => o.Pharmacy)
                .Include(o => o.CustomerOrderDetails)
                    .ThenInclude(d => d.Medicine)
                .FirstOrDefaultAsync(o => o.CustomerOrderId == id);
        }

        // Adds a new customer order
        public async Task Add(CustomerOrder customerOrder)
        {
            await context.customerOrders.AddAsync(customerOrder);
            await context.SaveChangesAsync();
        }

        // Saves changes made to a customer order
        public async Task CustomerOrderUpdate()
        {
            await context.SaveChangesAsync();
        }

        // Deletes a customer order
        public async Task CustomerOrderDelete(CustomerOrder customerOrder)
        {
            context.customerOrders.Remove(customerOrder);
            await context.SaveChangesAsync();
        }
    }
}
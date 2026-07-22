using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class CustomerOrderRepo
    {
        private PharmacyContext context;

        public CustomerOrderRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Returns all customer orders with related data
        public async Task<List<CustomerOrder>> GetAllCustomerOrders()
        {
            return await context.customerOrders
                .Include(o => o.Customer)
                .Include(o => o.Pharmacy)
                // Loads the Medicine related to each customer order detail
                .Include(o => o.CustomerOrderDetails).ThenInclude(d => d.Medicine)
                .ToList();
        }

        // Returns one customer order using its ID
        public async Task<CustomerOrder?> GetCustomerOrderById(int id)
        {
            return await context.customerOrders
                .Include(o => o.Customer)
                .Include(o => o.Pharmacy)
                // Loads the Medicine related to each customer order detail
                .Include(o => o.CustomerOrderDetails).ThenInclude(d => d.Medicine)
                .FirstOrDefault(o => o.CustomerOrderId == id);
        }

        // Adds  new customer order
        public async Task Add(CustomerOrder customerOrder)
        {
            await context.customerOrders.Add(customerOrder);
            await context.SaveChanges();
        }

        // Saves changes made to  customer order
        public async Task CustomerOrderUpdate()
        {
            await context.SaveChanges();
        }

        // Deletes  customer order
        public async Task CustomerOrderDelete(CustomerOrder customerOrder)
        {
            context.customerOrders.Remove(customerOrder);
            await context.SaveChanges();
        }
    }
}
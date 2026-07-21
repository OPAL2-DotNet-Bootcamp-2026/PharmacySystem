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
        public List<CustomerOrder> GetAllCustomerOrders()
        {
            return context.CustomerOrders
                .Include(o => o.Customer)
                .Include(o => o.Pharmacy)
                // Loads the Medicine related to each customer order detail
                .Include(o => o.CustomerOrderDetails).ThenInclude(d => d.Medicine)
                .ToList();
        }

        // Returns one customer order using its ID
        public CustomerOrder? GetCustomerOrderById(int id)
        {
            return context.CustomerOrders
                .Include(o => o.Customer)
                .Include(o => o.Pharmacy)
                // Loads the Medicine related to each customer order detail
                .Include(o => o.CustomerOrderDetails).ThenInclude(d => d.Medicine)
                .FirstOrDefault(o => o.CustomerOrderId == id);
        }

        // Adds  new customer order
        public void Add(CustomerOrder customerOrder)
        {
            context.CustomerOrders.Add(customerOrder);
            context.SaveChanges();
        }

        // Saves changes made to  customer order
        public void CustomerOrderUpdate()
        {
            context.SaveChanges();
        }

        // Deletes  customer order
        public void CustomerOrderDelete(CustomerOrder customerOrder)
        {
            context.CustomerOrders.Remove(customerOrder);
            context.SaveChanges();
        }
    }
}
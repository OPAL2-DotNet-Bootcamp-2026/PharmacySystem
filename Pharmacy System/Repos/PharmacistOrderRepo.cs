using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class PharmacistOrderRepo
    {
        private PharmacyContext context;

        public PharmacistOrderRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Returns all pharmacist orders with related data
        public List<PharmacistOrder> GetAllPharmacistOrders()
        {
            return context.pharmacistOrders
                .Include(o => o.Pharmacist)
                .Include(o => o.Pharmacy)
                .Include(o => o.PharmacistOrderDetails).ThenInclude(d => d.Medicine)
                .ToList();
        }

        // Returns one pharmacist order using its ID
        public PharmacistOrder? GetPharmacistOrderById(int id)
        {
            return context.pharmacistOrders
                .Include(o => o.Pharmacist)
                .Include(o => o.Pharmacy)
                .Include(o => o.PharmacistOrderDetails).ThenInclude(d => d.Medicine)
                .FirstOrDefault(o => o.PharmacistOrderId == id);
        }

        // Adds new pharmacist order
        public void Add(PharmacistOrder pharmacistOrder)
        {
            context.PharmacistOrders.Add(pharmacistOrder);
            context.SaveChanges();
        }

        // Save changes made to  pharmacist order
        public void PharmacistOrderUpdate()
        {
            context.SaveChanges();
        }

        // Delete  pharmacist order
        public void PharmacistOrderDelete(PharmacistOrder pharmacistOrder)
        {
            context.pharmacistOrders.Remove(pharmacistOrder);
            context.SaveChanges();
        }
    }
}
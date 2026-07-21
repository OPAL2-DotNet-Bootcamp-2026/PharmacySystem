using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;

namespace Pharmacy_System.Repos
{
    public class PaymentRepo
    {
        private PharmacyContext context;

        public PaymentRepo (PharmacyContext _context)
        {
            context = _context;
        }

        public async Task<List<Payment>> GetAllPayments()
        {
            return await context.payments.ToListAsync();
        }

        public async Task<Payment?> GetPaymentById(int id)
        {
            return await context.payments.FirstOrDefaultAsync(p => p.PaymentID == id);
        }

        public async Task<List<Payment>> GetPaymentsByOrder(int id)
        {
            return await context.payments.Where(p => p.CustomerOrderID == id).ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByStatus(string status)
        {
            return await context.payments.Where(p => p.Status == status).ToListAsync();
        }

        public async Task Add(Payment payment)
        {
            await context.payments.AddAsync(payment);
            await context.SaveChangesAsync();
        }

        public async Task Update()
        {
            await context.SaveChangesAsync();
        }

    }
}

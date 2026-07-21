using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;

namespace Pharmacy_System.Repos
{
    public class TransferRepo
    {
        private PharmacyContext context;

        public TransferRepo(PharmacyContext _context)
        {
            context = _context;
        }

        // Returns all transfers with related data
        public List<Transfer> GetAllTransfer()
        {
            return context.Transfers
                .Include(t => t.Warehouse)
                .Include(t => t.Pharmacy)
                .Include(t => t.PharmacistOrder)
                // Loads transfer details, then loads the medicine inside each detail
                .Include(t => t.TransferDetails)
                    .ThenInclude(d => d.Medicine)
                .ToList();
        }

        // Returns one transfer by ID with related data
        public Transfer? GetTransferById(int id)
        {
            return context.Transfers
                .Include(t => t.Warehouse)
                .Include(t => t.Pharmacy)
                .Include(t => t.PharmacistOrder)
                // Loads transfer details, then loads the medicine inside each detail
                .Include(t => t.TransferDetails)
                    .ThenInclude(d => d.Medicine)
                .FirstOrDefault(t => t.TransferId == id);
        }

        // Adds a new transfer with its transfer details
        public void Add(Transfer transfer)
        {
            context.Transfers.Add(transfer);
            context.SaveChanges();
        }

        // Saves changes made to an existing transfer
        public void TransferUpdate()
        {
            context.SaveChanges();
        }

        // Deletes an existing transfer
        public void TransferDelete(Transfer transfer)
        {
            context.Transfers.Remove(transfer);
            context.SaveChanges();
        }
    }
}
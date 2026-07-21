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

        // Returns all transfer  with related data
        public List<Transfer> GetAllTransfer()
        {
            return context.Transfers
                .Include(t => t.Warehouse)
                .Include(t => t.Pharmacy)
                .Include(t => t.Medicines)
                .ToList();
        }

        // Returns one transfer by  ID with related data
        public Transfer? GetTransferById(int id)
        {
            return context.Transfers
                .Include(t => t.Warehouse)
                .Include(t => t.Pharmacy)
                .Include(t => t.Medicines)
                .FirstOrDefault(t => t.TransferId == id);
        }

        // Adds  new transfer 
        public void Add(Transfer transfer)
        {
            context.Transfers.Add(transfer);
            context.SaveChanges();
        }

        // Saves changes made to existing transfer
        public void TransferUpdate()
        {
            context.SaveChanges();
        }

        // Deletes  existing transfer 
        public void TransferDelete(Transfer transfer)
        {
            context.Transfers.Remove(transfer);
            context.SaveChanges();
        }
    }
}
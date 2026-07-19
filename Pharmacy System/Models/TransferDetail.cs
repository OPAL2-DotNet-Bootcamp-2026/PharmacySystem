using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.Models
{
    public class TransferDetail
    {
        public int TransferId { get; set; }

        public Transfer Transfer { get; set; } = null!;

        public int MedicineId { get; set; }

        public Medicine Medicine { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
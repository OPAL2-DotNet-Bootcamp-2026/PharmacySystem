using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Transfer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransferId { get; set; }

        // Foreign key for the warehouse sending the medicines
        [Range(1, int.MaxValue)]
        public int WarehouseID { get; set; }

        // Navigation property for the source warehouse
        public Warehouse Warehouse { get; set; } = null!;

        // Foreign key for the pharmacy receiving the medicines
        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }

        // Navigation property for the destination pharmacy
        public Pharmacy Pharmacy { get; set; } = null!;

        // Date when the transfer was created
        public DateTime TransferDate { get; set; } = DateTime.Now;

        // Date when the pharmacy received the transfer
        public DateTime? ReceiveDate { get; set; }

        // Current status of the transfer
        [Required]
        public string Status { get; set; } = "Pending";

        // Many-to-Many relationship with Medicine
        public ICollection<Medicine> Medicines { get; set; }
            = new List<Medicine>();
    }
}
using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Transfer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransferId { get; set; }


        // FROM WAREHOUSE
        [Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; } = null!;


        // TO PHARMACY
        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }

        public Pharmacy Pharmacy { get; set; } = null!;


        public DateTime TransferDate { get; set; } = DateTime.Now;

        public DateTime? ReceiveDate { get; set; }


        [Required]
        public string Status { get; set; } = "Pending";




        // Many-to-Many relationship with Medicine
        public ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
    }
}
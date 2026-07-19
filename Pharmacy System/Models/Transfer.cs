using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Models
{
    public class Transfer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransferId { get; set; }

        //FROM WAREHOUSE
        [Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }//FK 

        public Warehouse Warehouse { get; set; } = null!;//RELATION

        //TO PHARMACY
        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }//FK

        public Pharmacy Pharmacy { get; set; } = null!;//RELATION

        public DateTime TransferDate { get; set; } = DateTime.Now;

        public DateTime? ReceiveDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public ICollection<TransferDetail> TransferDetails { get; set; } = new List<TransferDetail>();
           
    }
}
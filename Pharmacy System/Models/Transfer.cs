using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pharmacy_System.Models;


namespace Pharmacy_System.Modules
{
    public class Transfer : BaseEntity
    {
        // system Generated
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransferId { get; set; }

        // Input ==> selected warehouse "sending" the medicines
        [ForeignKey(nameof(Warehouse))]
        [Range(1, int.MaxValue)]
        public int WarehouseID { get; set; }
        public Warehouse Warehouse { get; set; } = null!;


        // input ==> selected pharmacy "receiving" the medicines
        [ForeignKey(nameof(Pharmacy))]
        [Range(1, int.MaxValue)]
        public int PharmacyID { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;


     
        // pharmacist order fulfilled by this transfer
        [ForeignKey(nameof(PharmacistOrder))]
        public int PharmacistOrderId { get; set; }
        public PharmacistOrder PharmacistOrder { get; set; }


        // Generated automatically when  transfer  created
        public DateTime TransferDate { get; set; } = DateTime.Now;

        // Updated when the pharmacy receives the transfer
        //will be Null when transfer not received
        public DateTime? ReceiveDate { get; set; }

        //  default value
        // Can be updated to Shipped, Received or Cancelled
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        // Contains the medicines and quantities included in the transfer
        public ICollection<TransferDetail> TransferDetails { get; set; } = new List<TransferDetail>();
           
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pharmacy_System.Models;

namespace Pharmacy_System.Modules
{
    public class PharmacistOrder : BaseEntity
    {
        // system generated
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacistOrderId { get; set; }

     
        [ForeignKey(nameof(Pharmacist))]
        [Range(1, int.MaxValue)]
        public int PharmacistID { get; set; }
        public Pharmacist Pharmacist { get; set; } = null!;


        [ForeignKey(nameof(Pharmacy))]
        [Range(1, int.MaxValue)]
        public int PharmacyID { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;


        // generated  when  order is created
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Calculated by  system from all order details
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.00", "99999999.99")]
        public decimal TotalCost { get; set; }

        //  default value
        // Can updated to Approved or Cancelled
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        // Contains all medicines requested in this order
        public ICollection<PharmacistOrderDetail> PharmacistOrderDetails { get; set; }= new List<PharmacistOrderDetail>();
            

        // One pharmacist order can be fulfilled by one or more transfers
        public ICollection<Transfer> Transfers { get; set; }= new List<Transfer>();
            
    }
}
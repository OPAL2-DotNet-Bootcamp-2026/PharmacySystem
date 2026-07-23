using Pharmacy_System.Models.Enums;
using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Models
{
    public class Payment : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }                      // System Generated

        public int CustomerOrderID {  get; set; }               // Foreign Key
        [Required, ForeignKey("CustomerOrderID")]
        public CustomerOrder customerOrder { get; set; }

        [Required]
        public decimal Amount { get; set; }                      

        [Required, MaxLength(30)]
        public PaymentMethod Method { get; set; }  

        [Required, MaxLength(30)]
        public PaymentStatus Status { get; set; }

        public DateTime? PaidDate { get; set; }

        [MaxLength(200)]
        public string? RefundReason { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundedAmount { get; set; }

        public DateTime? RefundedDate { get; set; }
    }
}

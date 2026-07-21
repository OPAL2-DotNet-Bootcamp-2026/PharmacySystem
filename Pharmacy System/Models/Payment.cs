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
        public string Method { get; set; }  

        [Required, MaxLength(30)]
        public string Status { get; set; }

        public DateTime? PaidDate { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Models
{
    public class CustomerOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerOrderId { get; set; }

        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }//FK

        public Customer Customer { get; set; } = null!;//RELATION

        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }//FK

        public Pharmacy Pharmacy { get; set; } = null!;//RELATION

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalCost { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        //RELATION
        public ICollection<CustomerOrderDetail> CustomerOrderDetails{ get; set; } = new List<CustomerOrderDetail>();
        
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class CustomerOrder : BaseEntity
    {
        // system Generated 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerOrderId { get; set; }

        // input ==> selected customer
        [ForeignKey(nameof(Customer))]
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;


        // input==> selected pharmacy fulfilling the order
         [ForeignKey(nameof(Pharmacy))]
        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }
        public Pharmacy Pharmacy { get; set; } = null!;

        //system generated  when order is created
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Calculated
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.00", "999999.99")]
        public decimal TotalCost { get; set; }


        // default value: Can be updated to :Completed or Cancelled
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        // Contains all medicines included in customer order
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; }= new List<CustomerOrderDetail>();
            
    }
}
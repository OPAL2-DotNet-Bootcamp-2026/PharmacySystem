using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Models
{
    public class PharmacistOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacistOrderId { get; set; }

        [Range(1, int.MaxValue)]
        public int PharmacistId { get; set; }//fk

        public Pharmacist Pharmacist { get; set; } = null!;//relation

        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }//fk

        public Pharmacy Pharmacy { get; set; } = null!;//relation

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public string Status { get; set; } = "Pending";

        public ICollection<PharmacistOrderDetail> PharmacistOrderDetails{ get; set; } = new List<PharmacistOrderDetail>();
        
    }
}
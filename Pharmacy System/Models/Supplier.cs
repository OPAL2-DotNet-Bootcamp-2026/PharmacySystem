using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierID { get; set; }  // system generation
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } // input
        [MaxLength(12)]
        [Required]
        public string Phone { get; set; } // input
        [Required]
        [MaxLength(100)]
        [RegularExpression(@".+@.+\.com$", ErrorMessage = "Email must contain @ and end with .com")]
        public string Email { get; set; } //input
        [Required]
        [MaxLength(150)]
        public string Location { get; set; } //input
        [Required]
        public bool IsActive { get; set; } = true; //Default value
        public ICollection<Supply> Supplies { get; set; } = new List<Supply>();

    }
}

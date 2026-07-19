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

        [Required]
        public string Phone { get; set; } // input
        [Required]
        [MaxLength(100)]
        [RegularExpression(@".+@.+\.com$", ErrorMessage = "Email must contain @ and end with .com")]
        public string Email { get; set; } //input
   


    }
}

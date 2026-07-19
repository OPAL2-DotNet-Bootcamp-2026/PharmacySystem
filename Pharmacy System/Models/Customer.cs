using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }             // System Generated

        [Required, MaxLength(150)]
        public string FullName { get; set; }            // User Input

        [Required, MaxLength(150)]
        public string Phone {  get; set; }              // User Input

        [Required]
        public DateOnly DOB { get; set; }               // System Auto Value
    }
}

using Pharmacy_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Customer : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }             // System Generated

        [Required, MaxLength(100)]
        public string FullName { get; set; }            // User Input

        [Required, MaxLength(8)]
        public string Phone {  get; set; }              // User Input

        [MaxLength(100)]
        public string? Email { get; set; }              // User Input

        [Required]
        public DateOnly DOB { get; set; }               // User Input

        [Required]
        public bool IsActive { get; set; } = true;      // System Default Value
    }
}

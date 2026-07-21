using Pharmacy_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Pharmacy : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacyID { get; set; }                 // System Generated

        [Required, MaxLength(150)]
        public string PharmacyName { get; set; }            // User Input

        [Required, MaxLength(150)]
        public string Location { get; set; }                // User Input        

        [Required, MaxLength(12)]
        public string Phone {  get; set; }                  // User Input

        [Required]
        public int StorageCapacity { get; set; }            // 

        [Required]
        public bool IsActive { get; set; } = true;          // System Default Value
    }
}

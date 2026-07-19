using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Pharmacy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacyID { get; set; }         // System Generated

        [Required, MaxLength(250)]
        public string Location { get; set; }        // User Input

        [Required, MaxLength(150)]
        public string Email { get; set; }           // User Input

        [Required, MaxLength(8)]
        public string Phone {  get; set; }          // User Input

        [Required]
        public int StorageCapacity { get; set; }    
    }
}

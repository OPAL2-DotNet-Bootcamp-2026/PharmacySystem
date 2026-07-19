using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Pharmacy
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacyID { get; set; }

        [Required, MaxLength(250)]
        public string Location { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required, MaxLength(8)]
        public string Phone {  get; set; }

        [Required]
        public int StorageCapacity { get; set; }
    }
}

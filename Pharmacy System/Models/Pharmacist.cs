using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Pharmacist
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacistID { get; set; }

        public int PharmacyID { get; set; }
        [Required, ForeignKey("PharmacyID")]
        public Pharmacy pharmacy { get; set; }

        [Required, MaxLength(150)]
        public string FullName { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required, MaxLength(8)]
        public string Phone {  get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Models
{
    [Index(nameof(PharmacyID), nameof(MedicineID), IsUnique = true)]
    public class PharmacyStock : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacyStockID { get; set; }

        public int PharmacyID { get; set; }
        [Required, ForeignKey("PharmacyID")]
        public Pharmacy pharmacy { get; set; }

        public int MedicineID { get; set; }
        [Required, ForeignKey("MedicineID")]
        public Medicine medicine { get; set; }

        [Required]
        public int Quantity { get; set; }

        public DateTime? ExpiryDate { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    [Index(nameof(UserID), IsUnique = true)]
    [Index(nameof(Phone), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class Pharmacist : BaseEntity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PharmacistID { get; set; }               // System Generated

        public int UserID { get; set; }                     // Foreign Key
        [Required, ForeignKey("UserID")]
        public User user { get; set; }

        public int PharmacyID { get; set; }                 // Foreign Key
        [Required, ForeignKey("PharmacyID")]
        public Pharmacy pharmacy { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }                // User Input

        [Required, MaxLength(13)]
        public string Phone {  get; set; }                  // User Input


        [Required, MaxLength(100)]
        public string Email { get; set; }                   // User Input

        [Required]
        public bool IsActive { get; set; } = true;          // System Default Value

    }
}

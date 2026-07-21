using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : BaseEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; } // system generated
        [Required]
        [MaxLength(100)]
        [RegularExpression(@".+@.+\.com$", ErrorMessage = "Email must contain @ and end with .com")]
        public string Email { get; set; } // user input
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } //user input
        [Required]
        [MaxLength(30)]
        public string Role { get; set; } //   ((Admin / Manager / Pharmacist))
        [Required]
        public bool IsActive { get; set; } = true; // default value


        // One User may have one Pharmacist profile , Because Admin is also a User, but Admin does not have a Pharmacist record.
        public Pharmacist? Pharmacist { get; set; }
    }
}

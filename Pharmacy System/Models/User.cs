using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]

    public class User : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }


        [Required]
        [MaxLength(100)]
        [RegularExpression(
            @".+@.+\.com$",
            ErrorMessage = "Email must contain @ and end with .com"
        )]
        public string Email { get; set; } = string.Empty;


        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;


        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;


        [Required]
        [MaxLength(30)]
        public string Role { get; set; } = string.Empty;


        [Required]
        public bool IsActive { get; set; } = true;


        public Pharmacist? Pharmacist { get; set; }


        [Required]
        public int FailedLoginAttempts { get; set; } = 0;


        public DateTime? LockedUntil { get; set; }
    }
}
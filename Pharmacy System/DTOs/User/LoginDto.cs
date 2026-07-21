using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.User
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@".+@.+\.com$", ErrorMessage = "Email must contain @ and end with .com")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(255)]
        public string PasswordHash { get; set; } = string.Empty;


    }
}

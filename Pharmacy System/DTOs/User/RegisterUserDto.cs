using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.User
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@".+@.+\.com$", ErrorMessage = "Email must contain @ and end with .com")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required")]
        [MaxLength(30)]
        [RegularExpression(
            @"^(Admin|Manager|Pharmacist)$",
            ErrorMessage = "Role must be Admin, Manager, or Pharmacist")]
        public string Role { get; set; } = string.Empty;
    }

}
}

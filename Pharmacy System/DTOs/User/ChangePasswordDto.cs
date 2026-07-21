using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.User
{
    public class ChangePasswordDto
    {

        [Required(ErrorMessage = "Old password is required")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [MinLength(8, ErrorMessage = "New password must contain at least 8 characters")]
        [MaxLength(255)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare(
            nameof(NewPassword),
            ErrorMessage = "The passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;



    }
}

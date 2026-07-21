using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Supplier
{
    public class CreateSupplierDto
    {
        [Required(ErrorMessage = "Supplier name is required")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number is required")]
        [MaxLength(13)]
        public string Phone { get; set; } = string.Empty;
        [MaxLength(100)]
        [RegularExpression(@".+@.+\.com$", ErrorMessage = "Email must contain @ and end with .com")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Location is required")]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;




}
}

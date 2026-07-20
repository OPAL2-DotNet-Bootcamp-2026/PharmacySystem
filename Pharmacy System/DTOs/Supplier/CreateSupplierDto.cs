using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Supplier
{
    public class CreateSupplierDto
    {
        [Required]
        [MaxLength(100)]
        public string SupplierName { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [MaxLength(100)]
        [RegularExpression(@".+@.+\.com$", ErrorMessage = "Email must contain @ and end with .com")]
        public string Email { get; set; } = string.Empty;
        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;




}
}

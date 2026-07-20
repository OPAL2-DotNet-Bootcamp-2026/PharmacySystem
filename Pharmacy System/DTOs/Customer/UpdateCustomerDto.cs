using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Customer
{
    public class UpdateCustomerDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateOnly DOB { get; set; }
    }
}

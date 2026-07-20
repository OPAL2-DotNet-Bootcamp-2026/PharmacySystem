using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DOTs.Pharmacist
{
    public class UpdatePharmacistDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public int? PharmacyID { get; set; }
    }
}

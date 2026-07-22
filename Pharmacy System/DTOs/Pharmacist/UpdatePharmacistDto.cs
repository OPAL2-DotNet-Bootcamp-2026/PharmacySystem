using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Pharmacist
{
    public class UpdatePharmacistDto
    {
        [
            Required(ErrorMessage = "The Full Name Is Required!!"),
            MaxLength(100, ErrorMessage = "The Maxlength Is 100 Char!!")
        ]
        public string FullName { get; set; } = string.Empty;

        [
            Required(ErrorMessage = "The Phone Is Required!!"),            
            RegularExpression(@"^\+968 \d{8}$", ErrorMessage = "Phone must be in the format +968 99998888")
        ]
        public string Phone { get; set; } = string.Empty;

        [
            Required(ErrorMessage = "Email Is Required!!"),
            MaxLength(100, ErrorMessage = "Email Cano't Be Exceed 100 Lenght!!"),
            EmailAddress(ErrorMessage = "Enter a Valid Email Format!!")
        ]
        public string Email { get; set; } = string.Empty;

        [
            Range(1, int.MaxValue, ErrorMessage = "A Valid Pharmacy Must be Selected")
        ]
        public int PharmacyID { get; set; }
    }
}

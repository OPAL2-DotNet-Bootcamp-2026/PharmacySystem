using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.DTOs.Pharmacist
{
    public class CreatePharmacistDto
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
            Required(ErrorMessage = "Password Is Required!!"), 
            MinLength(8, ErrorMessage = "Password Must Be At Least 8 Char!!")
        ]
        public string Password { get; set; }

        [
            Range(1, int.MaxValue, ErrorMessage = "A Valid Pharmacy Must be Selected")
        ]
        public int PharmacyID { get; set; }
    }
}

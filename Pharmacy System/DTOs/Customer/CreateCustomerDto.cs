using System.ComponentModel.DataAnnotations;
using Pharmacy_System.Validations;

namespace Pharmacy_System.DTOs.Customer
{
    public class CreateCustomerDto
    {
        [
            Required(ErrorMessage = "The Full Name Is Required!!"), 
            MaxLength(100, ErrorMessage = "The MaxLenght of Full Name Is 100 Char!!")
        ]
        public string FullName { get; set; }

        [
            Required(ErrorMessage = "The Phone Is Required!!"),
            RegularExpression(@"^\+968 \d{8}$", ErrorMessage = "Phone must be in the format +968 99998888")
        ]
        public string Phone { get; set; }

        [
            MaxLength(100, ErrorMessage = "Email Cano't Be Exceed 100 Lenght!!"), 
            EmailAddress(ErrorMessage = "Enter a Valid Email Format!!")
        ]
        public string? Email { get; set; }

        [
            Required(ErrorMessage = "The DOB Is Required!!"), 
            PastDate(ErrorMessage = "The DOB Can't Be In the Future!!")
        ]
        public DateOnly? DOB { get; set; }
    }    
}

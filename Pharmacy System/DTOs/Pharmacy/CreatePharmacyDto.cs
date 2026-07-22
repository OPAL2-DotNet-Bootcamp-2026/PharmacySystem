using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Pharmacy
{
    public class CreatePharmacyDto
    {
        [
            Required(ErrorMessage = "The Pharmacy name Is Required!!"), 
            MaxLength(150, ErrorMessage = "The Maxlenght Is 150 Char!!")
        ]
        public string PharmacyName { get; set; } = string.Empty;

        [
            Required(ErrorMessage = "The Location Is Required!!"), 
            MaxLength(150, ErrorMessage = "The Maxlenght Is 150 Char!!")
        ]
        public string Location { get; set; } = string.Empty;

        [
            Required(ErrorMessage = "The Phone Is Required!!"),
            RegularExpression(@"^\+968 \d{8}$", ErrorMessage = "Phone must be in the format +968 99998888")
        ]
        public string Phone { get; set; } = string.Empty;

        [
            Range(1, int.MaxValue, ErrorMessage = "The Storage Capacity must be greater than zero!!")
        ]
        public int StorageCapacity { get; set; }
    }
}

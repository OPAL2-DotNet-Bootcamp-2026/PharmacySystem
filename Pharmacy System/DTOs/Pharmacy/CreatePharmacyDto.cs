using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DOTs.Pharmacy
{
    public class CreatePharmacyDto
    {
        [Required]
        public string PharmacyName { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Phone {  get; set; } = string.Empty;

        public int StorageCapacity { get; set; }
    }
}

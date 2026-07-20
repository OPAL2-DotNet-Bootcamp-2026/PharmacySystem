namespace Pharmacy_System.DTOs.Pharmacy
{
    public class UpdatePharmacyDto
    {
        public string PharmacyName { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int StorageCapacity { get; set; }
    }
}

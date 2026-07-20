namespace Pharmacy_System.DTOs.Pharmacist
{
    public class PharmacistDto
    {
        public int PharmacistID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int PharmacyID { get; set; }
        public string PharmacyName { get; set; } = string.Empty;
    }
}

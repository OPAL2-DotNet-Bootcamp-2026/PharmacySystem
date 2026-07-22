namespace Pharmacy_System.DTOs.Customer
{
    public class CustomerDto
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateOnly DOB { get; set; }
        public bool IsActive { get; set; }
    }
}

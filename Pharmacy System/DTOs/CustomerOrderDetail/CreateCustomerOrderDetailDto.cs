using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.CustomerOrderDetail
{
    // Represents one medicine inside a customer order
    public class CreateCustomerOrderDetailDto
    {
        // Inpu
        [Range(1, int.MaxValue, ErrorMessage = "A valid medicine must be selected")]
        public int MedicineID { get; set; }

        // Input
        [Range(1, int.MaxValue,  ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }
}
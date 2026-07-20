using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.CustomerOrderDetail
{
    // Represents one medicine entry inside  customer order.
    // The full customer order contains a list of these entries,
    // so it can include multiple different medicines

    public class CreateCustomerOrderDetailDto
    {
        // ID of the medicine 
        [Range(1, int.MaxValue)]
        public int MedicineId { get; set; }

        // Quantity requested from this medicine
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
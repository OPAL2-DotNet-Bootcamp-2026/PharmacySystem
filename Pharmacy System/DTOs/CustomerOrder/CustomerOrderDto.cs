using Pharmacy_System.DTOs.CustomerOrderDetail;

namespace Pharmacy_System.DTOs.CustomerOrder
{
    // Used to return complete customer order information from the API
    public class CustomerOrderDto
    {
     
        public int CustomerOrderId { get; set; }

        // ID of the customer who placed  order
        public int CustomerId { get; set; }

        // Customer name displayed in the response
        public string CustomerName { get; set; } = string.Empty;

        // ID of the pharmacy handling the order
        public int PharmacyId { get; set; }

        // Date when the order  created
        public DateTime OrderDate { get; set; }

        // Total cost of all medicine items in the order
        public decimal TotalCost { get; set; }

        // Current status of  order
        public string Status { get; set; } = string.Empty;

        // List of all medicine items included in the order
        public List<CustomerOrderDetailDto> OrderDetails { get; set; }= new List<CustomerOrderDetailDto>();
            
    }
}
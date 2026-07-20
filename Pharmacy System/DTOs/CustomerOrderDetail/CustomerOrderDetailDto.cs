namespace Pharmacy_System.DTOs.CustomerOrderDetail
{
    // Represents the details of one medicine item inside  customer order
    // The full customer order can contain multiple medicine items
    public class CustomerOrderDetailDto
    {
        // ID of the medicine in this order item
        public int MedicineId { get; set; }

        // Name of the medicine displayed in the response
        public string MedicineName { get; set; } = string.Empty;

        // Quantity ordered from this medicine
        public int Quantity { get; set; }

        // Price of one medicine unit at the time of the order
        public decimal UnitPrice { get; set; }

        // Total price for this medicine item
        public decimal Subtotal { get; set; }
    }
}
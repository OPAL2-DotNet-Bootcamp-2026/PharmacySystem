
namespace Pharmacy_System.DTOs.CustomerOrderDetail
{
    // Used to return one medicine item inside a customer order
    public class CustomerOrderDetailDto
    {
        // Medicine information
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;

       
        public int Quantity { get; set; }

        // Price of one medicine unit at the time of ordering
        public decimal UnitPrice { get; set; }

        // Calculated ==> UnitPrice × Quantity
        public decimal Subtotal { get; set; }
    }
}
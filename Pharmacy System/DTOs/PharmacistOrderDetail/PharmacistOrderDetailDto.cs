namespace Pharmacy_System.DTOs.PharmacistOrderDetail
{
    // Represents the details of  medicine item inside  pharmacist order
    // The full pharmacist order can contain multiple medicine 
    public class PharmacistOrderDetailDto
    {
        // ID of the medicine in this order item
        public int MedicineId { get; set; }

        // Name of the medicine displayed in the response
        public string MedicineName { get; set; } = string.Empty;

        // Quantity requested from this medicine
        public int Quantity { get; set; }
    }
}
namespace Pharmacy_System.DTOs.PharmacistOrderDetail
{
    //  return one medicine  inside a pharmacist order
    public class PharmacistOrderDetailDto
    {
        // Medicine information
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;

  
        public int Quantity { get; set; }
    }
}
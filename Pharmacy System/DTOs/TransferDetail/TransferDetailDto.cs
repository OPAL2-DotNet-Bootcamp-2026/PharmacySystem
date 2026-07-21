namespace Pharmacy_System.DTOs.TransferDetail
{
    public class TransferDetailDto
    {
        public int MedicineID { get; set; }

        public string MedicineName { get; set; } = string.Empty;

        public int Quantity { get; set; }
    }
}
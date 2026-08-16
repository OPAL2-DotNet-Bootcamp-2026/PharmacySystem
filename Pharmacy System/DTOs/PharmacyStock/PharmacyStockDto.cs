namespace Pharmacy_System.DTOs.PharmacyStock
{
    public class PharmacyStockDto
    {
        public int PharmacyStockID { get; set; }

        public int PharmacyID { get; set; }        

        public int MedicineID { get; set; }

        public string MedicineName { get; set; } = string.Empty;


        public int Quantity { get; set; }

        public DateOnly ExpiryDate { get; set; }
    }
}

namespace Pharmacy_System.DTOs.WarehouseStock
{
    public class WarehouseStockDto
    {

        public int WarehouseStockID { get; set; } // System generated

        public int WarehouseID { get; set; }

        public int MedicineID { get; set; }
        public string MedicineName { get; set; }

        public int Quantity { get; set; }

        public DateOnly? ExpiryDate { get; set; }




    }
}

namespace Pharmacy_System.DTOs.Supply
{
    //  return supply info from the API
    public class SupplyDto
    {
      
        public int SupplyId { get; set; }

        public string BatchNumber { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime SupplyDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        // Supplier information
        public int SupplierID { get; set; }
        public string FullName { get; set; } = string.Empty;

        // Medicine information
        public int MedicineID { get; set; }
        public string MedicineName { get; set; } = string.Empty;

        // Warehouse information
        public int WarehouseID { get; set; }
        public string Location { get; set; } = string.Empty;

        
    }
}
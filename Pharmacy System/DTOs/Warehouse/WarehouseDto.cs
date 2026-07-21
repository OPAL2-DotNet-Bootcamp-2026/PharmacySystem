namespace Pharmacy_System.DTOs.Warehouse
{
    public class WarehouseDto
    {
        public int WarehouseID { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}

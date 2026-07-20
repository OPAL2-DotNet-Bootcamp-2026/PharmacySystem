namespace Pharmacy_System.DOTs.Warehouse
{
    public class WarehouseDto
    {
        public int WarehouseId { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ExpiryDate { get; set; }

    }
}

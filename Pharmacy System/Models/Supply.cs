using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Supply
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplyId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalCost { get; set; }

        public DateTime SupplyDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "date")]
        public DateTime ExpiryDate { get; set; }

        // Supplier relationship
        public int SupplierID { get; set; }//FK
        public Supplier Supplier { get; set; } = null!;

        // Medicine relationship
        public int MedicineID { get; set; }//FK
        public Medicine Medicine { get; set; } = null!;

        // Warehouse relationship
        public int WarehouseID { get; set; }//FK
        public Warehouse Warehouse { get; set; } = null!;
    }
}
using Pharmacy_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Supply : BaseEntity
    {
      
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplyId { get; set; }

        // Input
        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        // Input
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Input: cost of one medicine unit
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "999999.99")]
        public decimal UnitCost { get; set; }

        // Calculated 
        // TotalCost = Quantity × UnitCost
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "999999999.99")]
        public decimal TotalCost { get; set; }

        public DateTime SupplyDate { get; set; } = DateTime.Now;

        
        [Column(TypeName = "date")]
        public DateTime ExpiryDate { get; set; }

        //  supplier
        [ForeignKey(nameof(Supplier))]
        [Range(1, int.MaxValue)]
        public int SupplierID { get; set; }

        public Supplier Supplier { get; set; } = null!;

        //  medicine
        [ForeignKey(nameof(Medicine))]
        [Range(1, int.MaxValue)]
        public int MedicineID { get; set; }

        public Medicine Medicine { get; set; } = null!;

        //  warehouse
        [ForeignKey(nameof(Warehouse))]
        [Range(1, int.MaxValue)]
        public int WarehouseID { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
    }
}
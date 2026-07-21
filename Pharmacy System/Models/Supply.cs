using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Supply
    {
        // Generated automatically by the database
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplyId { get; set; }

        // Input   
        // Identifies the supplied medicine batch
        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        // Input   
        // Number of medicine units supplied
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        //  calculated 
        // Total cost of the  supply
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "999999.99")]
        public decimal TotalCost { get; set; }

        // Generated automatically when  supply  created
        public DateTime SupplyDate { get; set; } = DateTime.Now;


        // Expiry date of  supplied batch
        [Required]
        [Column(TypeName = "date")]
        public DateTime ExpiryDate { get; set; }

      
        [ForeignKey(nameof(Supplier))]
        [Range(1, int.MaxValue)]
        public int SupplierID { get; set; }
        public Supplier Supplier { get; set; } = null!;


        [ForeignKey(nameof(Medicine))]
        [Range(1, int.MaxValue)]
        public int MedicineID { get; set; }
        public Medicine Medicine { get; set; } = null!;

        [ForeignKey(nameof(Warehouse))]
        [Range(1, int.MaxValue)]
        public int WarehouseID { get; set; }
        public Warehouse Warehouse { get; set; } = null!;
    }
}
using Microsoft.EntityFrameworkCore;
using Pharmacy_System.Modules;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Models
{
    [PrimaryKey(nameof(WarehouseID), nameof(MedicineID))]

    public class WarehouseStock : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WarehouseStockID { get; set; } // system generated

        // M WarehouseStocks to 1 Warehouse
        [Required]
        [ForeignKey(nameof(Warehouse))]
        public int WarehouseID { get; set; }
        public Warehouse Warehouse { get; set; }

        // M WarehouseStocks to 1 Medicine
        [Required]
        [ForeignKey(nameof(Medicine))]
        public int MedicineID { get; set; }
        public Medicine Medicine { get; set; }

        [Required]
        [Range(0, int.MaxValue)] //zero or more
        public int Quantity { get; set; }
        public DateTime? ExpiryDate { get; set; }

       

    }
}

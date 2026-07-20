using Pharmacy_System.Modeles;
using Pharmacy_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Medicine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicineID { get; set; }

        [Required(ErrorMessage = "Medicine name is required")]
        [MaxLength(100)]
        public string MedicineName { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0.01", "10000.00",
            ErrorMessage = "Unit price must be between 0.01 and 10,000")]
        public decimal UnitPrice { get; set; }

        public bool isAvailable { get; set; } = true;

        [Required]
        [MaxLength(100)]
        public string Category { get; set; }


        // Warehouse relationship
        [ForeignKey(nameof(Warehouse))]
        public int WarehouseId { get; set; }

        public Warehouse Warehouse { get; set; }


        // One-to-Many relationship with Supplies
        public ICollection<Supply> Supplies { get; set; } = new List<Supply>();


        // Orders containing this medicine
        public ICollection<PharmacistOrder> PharmacistOrders { get; set; } = new List<PharmacistOrder>();

        // Many-to-Many relationship with Transfers
        public ICollection<Transfer> Transfers { get; set; }= new List<Transfer>();
    


    }
}
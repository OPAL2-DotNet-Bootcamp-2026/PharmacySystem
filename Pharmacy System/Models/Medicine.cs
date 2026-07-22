using Pharmacy_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class Medicine : BaseEntity
    {
        // Generated automatically by the database
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MedicineID { get; set; }

        // Input
        [Required(ErrorMessage = "Medicine name is required")]
        [MaxLength(100,ErrorMessage = "Medicine name cannot exceed 100 characters")]
        public string MedicineName { get; set; } = string.Empty;

        // Input
        [Column(TypeName = "decimal(18,2)")]
        [Range(  typeof(decimal), "0.01", "99999999.99",ErrorMessage = "Unit price must be greater than 0")]
       
      
        public decimal UnitPrice { get; set; }

        //  default value
        // Can change depending on medicine availability
        public bool isAvailable { get; set; } = true;

        //  default value 
        public bool IsActive { get; set; } = true;

       
        // One category can contain many medicines
        [ForeignKey(nameof(Category))]
        [Range(1, int.MaxValue,ErrorMessage = "A valid category must be selected")]
       
        public int CategoryID { get; set; }

        // Navigation property loaded by Entity Framework
        public Category Category { get; set; } = null!;

        // One medicine can appear in many supply records
        public ICollection<Supply> Supplies { get; set; }= new List<Supply>();
            

        // One medicine can appear in many customer order details
        public ICollection<CustomerOrderDetail> CustomerOrderDetails { get; set; } = new List<CustomerOrderDetail>();
           

        // One medicine can appear in many pharmacist order details
        public ICollection<PharmacistOrderDetail> PharmacistOrderDetails { get; set; }= new List<PharmacistOrderDetail>();
            

        // One medicine can appear in many transfer details
        public ICollection<TransferDetail> TransferDetails { get; set; }= new List<TransferDetail>();
            

        // One medicine can have stock records in many warehouses
        public ICollection<WarehouseStock> WarehouseStocks { get; set; }  = new List<WarehouseStock>();
          

        // One medicine can have stock records in many pharmacies
        public ICollection<PharmacyStock> PharmacyStocks { get; set; }= new List<PharmacyStock>();
            
    }
}
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    // Composite primary key:
    // CustomerOrderId + MedicineID
    [PrimaryKey(nameof(CustomerOrderId), nameof(MedicineID))]

    public class CustomerOrderDetail
    {
        // Part 1 of the composite primary key
        // Foreign key for CustomerOrder
        [ForeignKey(nameof(CustomerOrder))]
        public int CustomerOrderId { get; set; }

        public CustomerOrder CustomerOrder { get; set; } = null!;

        // Part 2 of the composite primary key
        // Foreign key for Medicine
        [ForeignKey(nameof(Medicine))]
        [Range(1, int.MaxValue)]
        public int MedicineID { get; set; }

        public Medicine Medicine { get; set; } = null!;

        // Input: quantity selected in the order
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // System value: medicine price at order time
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // System calculated: Quantity × UnitPrice
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
    }
}
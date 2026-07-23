using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pharmacy_System.Models;

namespace Pharmacy_System.Modules
{
    // Composite primary key:
    // CustomerOrderId + MedicineID
    [PrimaryKey(nameof(CustomerOrderId), nameof(MedicineID))]
    public class CustomerOrderDetail : BaseEntity
    {
        // composite primary key
        [ForeignKey(nameof(CustomerOrder))]
        public int CustomerOrderId { get; set; }
        public CustomerOrder CustomerOrder { get; set; } = null!;

        // composite primary key
        [ForeignKey(nameof(Medicine))]
        [Range(1, int.MaxValue)]
        public int MedicineID { get; set; }
        public Medicine Medicine { get; set; } = null!;

        // Input: quantity selected in  order
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // System value: medicine price at order time
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        //  calculated ==> quantity × unitPrice
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
    }
}
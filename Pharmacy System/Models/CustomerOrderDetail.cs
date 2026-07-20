using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.Modules
{
    public class CustomerOrderDetail
    {
        public int CustomerOrderId { get; set; }

        public CustomerOrder CustomerOrder { get; set; } = null!;//relation

        public int MedicineID { get; set; }

        public Medicine Medicine { get; set; } = null!;//relation

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal Subtotal { get; set; }
    }
}
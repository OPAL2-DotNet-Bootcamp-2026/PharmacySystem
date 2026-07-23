using Pharmacy_System.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.DTOs.PharmacyStock
{
    public class AdjustPharmacyStockDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "A valid Pharmacy Id is required.")]
        public int PharmacyID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A valid Medicine Id is required.")]
        public int MedicineID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }
    }
}

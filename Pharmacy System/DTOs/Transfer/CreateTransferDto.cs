using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Transfer
{
    // Used to receive information when creating a new shipment
    public class CreateTransferDto
    {
        // ID of the warehouse sending the shipment
        [Range(1, int.MaxValue)]
        public int WarehouseId { get; set; }

        // ID of the pharmacy receiving the shipment
        [Range(1, int.MaxValue)]
        public int PharmacyId { get; set; }

        // IDs of medicines included in the shipment
        [Required]
        [MinLength(1)]
        public List<int> MedicineIds { get; set; }= new List<int>();
            
    }
}
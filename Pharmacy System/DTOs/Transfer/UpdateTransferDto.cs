using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Transfer
{
    // Used to update the shipment status and receiving date
    public class UpdateTransferDto
    {
        // Current shipment status
        [Required]
        public string Status { get; set; } = string.Empty;

        // Date when the pharmacy received the shipment
        public DateTime? ReceiveDate { get; set; }
    }
}
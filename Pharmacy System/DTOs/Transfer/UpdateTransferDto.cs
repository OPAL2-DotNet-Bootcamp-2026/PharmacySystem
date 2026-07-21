using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Transfer
{
    public class UpdateTransferDto
    {
        [Required(ErrorMessage = "Transfer status is required")]
        [MaxLength(30)]
        public string Status { get; set; } = string.Empty;

        public DateTime? ReceiveDate { get; set; }
    }
}
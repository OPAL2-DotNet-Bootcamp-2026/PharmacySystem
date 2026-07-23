using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Payment
{
    public class RefundDto
    {
        [Range(0.01, 1000000, ErrorMessage = "Refund amount must be greater than zero.")]
        public decimal RefundedAmount { get; set; }

        [
            Required(ErrorMessage = "Refund Reason is Required!!"), 
            MaxLength(200)
        ]
        public string RefundReason { get; set; } = string.Empty;

        [
            Required(ErrorMessage = "The Refund Date is Required!!")
        ]
        public DateTime RefundedDate { get; set; }
    }
}

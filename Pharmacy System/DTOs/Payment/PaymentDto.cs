using Pharmacy_System.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pharmacy_System.DTOs.Payment
{
    public class PaymentDto
    {
        public int PaymentID { get; set; }
        public int CustomerOrderID { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime? PaidDate { get; set; }
    }
}

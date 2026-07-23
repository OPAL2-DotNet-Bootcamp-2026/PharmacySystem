using Pharmacy_System.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy_System.DTOs.Payment
{
    public class CreatePaymentDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "A Valid Customer Order Id Is Required!!")]
        public int CustomerOrderID { get; set; }

        [Range(typeof(decimal), "0.01", "999999.99", ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment Method is Required!!")]
        public PaymentMethod Method { get; set; }
    }
}

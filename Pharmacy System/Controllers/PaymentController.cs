using Microsoft.AspNetCore.Mvc;
using Pharmacy_System.DTOs.Payment;
using Pharmacy_System.Models.Enums;
using Pharmacy_System.Services;

namespace Pharmacy_System.Controllers
{
    [ApiController, Route("api/[controller]")]    
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService paymentService;

        public PaymentController(PaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpGet]                                          // GET api/Payment
        public async Task<IActionResult> GetAll()
            => Ok(await paymentService.GetAll());

        [HttpGet("{id}")]                                  // GET api/Payment/5
        public async Task<IActionResult> GetById(int id)
        {
            PaymentDto? payment = await paymentService.GetById(id);
            if (payment == null) return NotFound();
            return Ok(payment);
        }

        [HttpPost]                                         // POST api/Payment
        public async Task<IActionResult> Add(CreatePaymentDto dto)
        {
            PaymentDto payment = await paymentService.Add(dto);
            return CreatedAtAction(nameof(GetById), new { id = payment.PaymentID }, payment);
        }

        [HttpGet("by-order/{orderId}")]                    // GET api/Payment/by-order/5
        public async Task<IActionResult> GetByOrder(int orderId)
            => Ok(await paymentService.GetByOrder(orderId));

        [HttpPatch("{id}/pay")]                            // PATCH api/Payment/5/pay
        public async Task<IActionResult> MarkPaid(int id)
        {
            bool ok = await paymentService.MarkPaid(id);
            if (!ok) return NotFound();
            return Ok("Payment marked as paid");
        }

        [HttpPost("{id}/refund")]                          // POST api/Payment/5/refund
        public async Task<IActionResult> Refund(int id, RefundDto dto)
        {
            bool ok = await paymentService.Refund(id, dto);
            if (!ok) return NotFound();
            return Ok("Payment refunded");
        }

        [HttpGet("by-status/{status}")]                    // GET api/Payment/by-status/Paid
        public async Task<IActionResult> GetByStatus(PaymentStatus status)
            => Ok(await paymentService.GetByStatus(status));
    }
}
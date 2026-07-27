using Pharmacy_System.Repos;
using Pharmacy_System.Models;
using Pharmacy_System.Models.Enums;
using Pharmacy_System.DTOs.Payment;

namespace Pharmacy_System.Services
{
    public class PaymentService
    {
        private readonly PaymentRepo paymentRepo;

        public PaymentService(PaymentRepo paymentRepo)
        {
            this.paymentRepo = paymentRepo;
        }

        public async Task<List<PaymentDto>> GetAll()
        {
            List<Payment> payments = await paymentRepo.GetAllPayments();
            return payments.Select(ToDto).ToList();
        }

        public async Task<PaymentDto?> GetById(int id)
        {
            Payment? payment = await paymentRepo.GetPaymentById(id);
            return payment == null ? null : ToDto(payment);
        }

        public async Task<PaymentDto> Add(CreatePaymentDto dto)
        {
            Payment payment = new Payment
            {
                CustomerOrderID = dto.CustomerOrderID,
                Amount = dto.Amount,
                Method = dto.Method,             
                Status = PaymentStatus.Pending,  
                PaidDate = null
            };

            await paymentRepo.Add(payment);
            return ToDto(payment);
        }

        public async Task<List<PaymentDto>> GetByOrder(int customerOrderId)
        {
            List<Payment> payments = await paymentRepo.GetPaymentsByOrder(customerOrderId);
            return payments.Select(ToDto).ToList();
        }

        public async Task<bool> MarkPaid(int id)
        {
            Payment? payment = await paymentRepo.GetPaymentById(id);

            if (payment == null) 
                return false;

            if (payment.Status != PaymentStatus.Pending)
                throw new Exception("Only a pending payment can be marked paid");

            payment.Status = PaymentStatus.Paid;
            payment.PaidDate = DateTime.UtcNow;

            await paymentRepo.Update();
            return true;
        }

        public async Task<bool> Refund(int id, RefundDto dto)
        {
            Payment? payment = await paymentRepo.GetPaymentById(id);

            if (payment == null) 
                return false;

            if (payment.Status != PaymentStatus.Paid)
                throw new Exception("Only a paid payment can be refunded");

            if (dto.RefundedAmount > payment.Amount)
                throw new Exception("Refund cannot exceed the amount paid");

            payment.Status = PaymentStatus.Refunded;
            payment.RefundedAmount = dto.RefundedAmount;
            payment.RefundReason = dto.RefundReason;
            payment.RefundedDate = DateTime.UtcNow;

            await paymentRepo.Update();
            return true;
        }

        public async Task<List<PaymentDto>> GetByStatus(PaymentStatus status)
        {
            List<Payment> payments = await paymentRepo.GetPaymentsByStatus(status);
            return payments.Select(ToDto).ToList();
        }

        private static PaymentDto ToDto(Payment p) => new PaymentDto
        {
            PaymentID = p.PaymentID,
            CustomerOrderID = p.CustomerOrderID,
            Amount = p.Amount,
            Method = p.Method,
            Status = p.Status,
            PaidDate = p.PaidDate
        };
    }
}

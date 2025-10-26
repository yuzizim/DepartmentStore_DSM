using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.Entities
{
    public class Payment : BaseEntity
    {
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = "Cash"; // or CreditCard, QR, etc.

        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? Status { get; set; } = "Completed";
    }
}

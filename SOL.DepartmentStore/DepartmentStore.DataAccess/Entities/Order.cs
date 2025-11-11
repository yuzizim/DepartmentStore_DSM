using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.DataAccess.Entities
{
    public enum OrderStatus
    {
        Draft, Pending, Confirmed, Processing, Shipped, Delivered, Cancelled, Returned
    }

    public class Order : BaseEntity
    {
        [ForeignKey(nameof(Customer))]
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [ForeignKey(nameof(Employee))]
        public Guid? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Draft;

        public ICollection<OrderDetail> OrderDetails { get; set; } = null;
        public Payment? Payment { get; set; }
    }
}
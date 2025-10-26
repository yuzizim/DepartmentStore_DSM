using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.Entities
{
    public class Order : BaseEntity
    {
        [ForeignKey("Customer")]
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [ForeignKey("Employee")]
        public Guid? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
        public Payment? Payment { get; set; }
    }
}

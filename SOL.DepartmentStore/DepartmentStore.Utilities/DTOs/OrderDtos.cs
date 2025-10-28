using System;
using System.Collections.Generic;

namespace DepartmentStore.Utilities.DTOs
{
    public class OrderDetailDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => Quantity * UnitPrice;
    }

    public class OrderDto
    {
        public Guid Id { get; set; }
        public string? CustomerName { get; set; }
        public string? EmployeeName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Draft";
        public List<OrderDetailDto> Details { get; set; } = new();
    }

    public class CreateOrderDto
    {
        public Guid CustomerId { get; set; }
        public List<CreateOrderDetailDto> OrderDetails { get; set; } = new();
        public string PaymentMethod { get; set; } = "Cash";
    }

    public class CreateOrderDetailDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public string Status { get; set; } = "Approved";
    }
}

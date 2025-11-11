using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.DataAccess.Entities
{
    public class OrderDetail : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public Order? Order { get; set; }
        public Product? Product { get; set; } // ← Đảm bảo có
    }
}
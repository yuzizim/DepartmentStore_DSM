using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.DataAccess.Entities
{
    public class Product : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey(nameof(Supplier))]
        public Guid SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public Inventory? Inventory { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
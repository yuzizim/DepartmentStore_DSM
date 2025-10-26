using DepartmentStore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.DataAccess.Entities
{
    public class Product : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        [ForeignKey("Supplier")]
        public Guid SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public Inventory? Inventory { get; set; }
    }
}

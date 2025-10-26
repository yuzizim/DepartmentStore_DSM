using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.Entities
{
    public class Inventory : BaseEntity
    {
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; } = 10;
        public DateTime LastRestockDate { get; set; } = DateTime.UtcNow;
    }
}

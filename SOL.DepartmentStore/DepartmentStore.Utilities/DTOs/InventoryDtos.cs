using System;

namespace DepartmentStore.Utilities.DTOs
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public DateTime LastRestockDate { get; set; }
    }

    public class UpdateInventoryDto
    {
        public int QuantityChange { get; set; } // positive to add, negative to remove
        public DateTime? RestockDate { get; set; }
    }
}

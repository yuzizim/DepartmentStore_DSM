using System;

namespace DepartmentStore.Utilities.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int QuantityOnHand { get; set; }        // from Inventory
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Guid SupplierId { get; set; }
        public string? SupplierName { get; set; }
    }

    public class CreateProductDto
    {
        public string SKU { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SupplierId { get; set; }
        public int InitialQuantity { get; set; } = 0;
    }

    public class UpdateProductDto
    {
        public string? SKU { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SupplierId { get; set; }
    }
}

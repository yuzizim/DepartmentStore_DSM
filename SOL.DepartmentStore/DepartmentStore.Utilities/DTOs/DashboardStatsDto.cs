// src/DepartmentStore.Utilities/DTOs/DashboardStatsDto.cs
namespace DepartmentStore.Utilities.DTOs
{
    public class DashboardStatsDto
    {
        public int UserCount { get; set; }
        public int ProductCount { get; set; }
        public int OrderCount { get; set; }
        public int PaymentCount { get; set; }
        public int SupplierCount { get; set; }
        public int CategoryCount { get; set; }
        public int EmployeeCount { get; set; }
        public int InventoryCount { get; set; }
        public int LowStockCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
// src/DepartmentStore.Service/Implementations/DashboardService.cs
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.DataAccess.Repositories;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Service.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository _userRepo;
        private readonly IBaseRepository<Product> _productRepo;
        private readonly IBaseRepository<Order> _orderRepo;
        private readonly IBaseRepository<Payment> _paymentRepo;
        private readonly IBaseRepository<Supplier> _supplierRepo;
        private readonly IBaseRepository<Category> _categoryRepo;
        private readonly IBaseRepository<Employee> _employeeRepo;
        private readonly IBaseRepository<Inventory> _inventoryRepo;

        public DashboardService(
            IUserRepository userRepo,
            IBaseRepository<Product> productRepo,
            IBaseRepository<Order> orderRepo,
            IBaseRepository<Payment> paymentRepo,
            IBaseRepository<Supplier> supplierRepo,
            IBaseRepository<Category> categoryRepo,
            IBaseRepository<Employee> employeeRepo,
            IBaseRepository<Inventory> inventoryRepo)
        {
            _userRepo = userRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _supplierRepo = supplierRepo;
            _categoryRepo = categoryRepo;
            _employeeRepo = employeeRepo;
            _inventoryRepo = inventoryRepo;
        }

        public async Task<DashboardStatsDto> GetAdminStatsAsync()
        {
            return new DashboardStatsDto
            {
                UserCount = await _userRepo.CountAsync(),
                ProductCount = await _productRepo.CountAsync(),
                OrderCount = await _orderRepo.CountAsync(),
                PaymentCount = await _paymentRepo.CountAsync()
            };
        }

        public async Task<DashboardStatsDto> GetManagerStatsAsync()
        {
            return new DashboardStatsDto
            {
                ProductCount = await _productRepo.CountAsync(),
                SupplierCount = await _supplierRepo.CountAsync(),
                CategoryCount = await _categoryRepo.CountAsync(),
                EmployeeCount = await _employeeRepo.CountAsync()
            };
        }

        public async Task<DashboardStatsDto> GetInventoryStatsAsync()
        {
            var lowStock = await _inventoryRepo.FindAsync(i => i.QuantityOnHand < i.ReorderLevel);
            return new DashboardStatsDto
            {
                InventoryCount = await _inventoryRepo.CountAsync(),
                LowStockCount = lowStock.Count
            };
        }

        public async Task<DashboardStatsDto> GetSalesStatsAsync(string employeeId)
        {
            if (!Guid.TryParse(employeeId, out Guid empGuid))
                return new DashboardStatsDto();

            var orders = await _orderRepo.FindAsync(o => o.EmployeeId == empGuid);
            return new DashboardStatsDto
            {
                OrderCount = orders.Count,
                Revenue = orders.Sum(o => o.TotalAmount)
            };
        }
    }
}
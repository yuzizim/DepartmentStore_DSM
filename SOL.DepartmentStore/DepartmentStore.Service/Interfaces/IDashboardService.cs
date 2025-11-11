using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Service.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetAdminStatsAsync();
        Task<DashboardStatsDto> GetManagerStatsAsync();
        Task<DashboardStatsDto> GetInventoryStatsAsync();
        Task<DashboardStatsDto> GetSalesStatsAsync(string employeeId);
    }
}

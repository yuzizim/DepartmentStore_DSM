// src/DepartmentStore.API/Controllers/DashboardController.cs
using DepartmentStore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DepartmentStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminStats() =>
            Ok(await _dashboardService.GetAdminStatsAsync());

        [HttpGet("manager")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetManagerStats() =>
            Ok(await _dashboardService.GetManagerStatsAsync());

        [HttpGet("inventory")]
        [Authorize(Roles = "Admin,Manager,InventoryEmployee")]
        public async Task<IActionResult> GetInventoryStats() =>
            Ok(await _dashboardService.GetInventoryStatsAsync());

        [HttpGet("sales/{employeeId}")]
        [Authorize(Roles = "Admin,Manager,SalesEmployee")]
        public async Task<IActionResult> GetSalesStats(string employeeId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdminOrManager = User.IsInRole("Admin") || User.IsInRole("Manager");

            if (!isAdminOrManager && userId != employeeId)
                return Forbid();

            return Ok(await _dashboardService.GetSalesStatsAsync(employeeId));
        }
    }
}
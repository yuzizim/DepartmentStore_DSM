using Microsoft.AspNetCore.Mvc.RazorPages;
using DepartmentStore.Client.Services;
using DepartmentStore.Utilities.DTOs;

namespace DepartmentStore.Client.Pages.Dashboard.Admin
{
    public class IndexModel : PageModel
    {
        private readonly ApiClientService _api;
        public DashboardStatsDto Stats { get; set; } = new();

        public IndexModel(ApiClientService api)
        {
            _api = api;
        }

        public async Task OnGetAsync()
        {
            Stats = await _api.GetAsync<DashboardStatsDto>("dashboard/admin");
        }
    }
}

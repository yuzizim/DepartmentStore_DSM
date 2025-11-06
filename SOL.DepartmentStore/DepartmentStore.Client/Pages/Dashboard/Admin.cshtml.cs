using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DepartmentStore.Client.Pages.Dashboard
{
    public class AdminModel : PageModel
    {
        public int UserCount { get; set; }
        public int ProductCount { get; set; }
        public int OrderCount { get; set; }
        public int PaymentCount { get; set; }

        public string[] MonthLabels { get; set; } = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };
        public int[] MonthlyOrders { get; set; } = new[] { 25, 40, 35, 50, 30, 45, 60, 55, 70 };

        public string[] RoleLabels { get; set; } = new[] { "Admin", "Manager", "Employee", "Customer" };
        public int[] RoleData { get; set; } = new[] { 3, 5, 12, 80 };

        public void OnGet()
        {
            // TODO: gọi API sau này
            UserCount = 100;
            ProductCount = 250;
            OrderCount = 1200;
            PaymentCount = 1150;
        }
    }
}

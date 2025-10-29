using DepartmentStore.Client.Models.Auth;
using DepartmentStore.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DepartmentStore.Client.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AuthService _auth;

        public LoginModel(AuthService auth)
        {
            _auth = auth;
        }

        [BindProperty]
        public LoginRequest LoginRequest { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var success = await _auth.LoginAsync(LoginRequest);
            if (!success)
            {
                ErrorMessage = "Sai tài khoản hoặc mật khẩu!";
                return Page();
            }

            return RedirectToPage("/Dashboard/Index");
        }
    }
}

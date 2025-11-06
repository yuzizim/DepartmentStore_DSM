using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace DepartmentStore.Client.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(IHttpClientFactory clientFactory, ILogger<LoginModel> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public class LoginInput
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
            public List<string> Roles { get; set; } = new();
        }

        public void OnGet()
        {
            // clear old session
            HttpContext.Session.Clear();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            try
            {
                var client = _clientFactory.CreateClient();
                client.BaseAddress = new Uri("https://localhost:7153/");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonSerializer.Serialize(Input), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/auth/login", content);

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Unauthorized => "Sai tên đăng nhập hoặc mật khẩu!",
                        System.Net.HttpStatusCode.NotFound => "Tài khoản không tồn tại!",
                        _ => "Đăng nhập thất bại!"
                    };
                    return Page();
                }

                var json = await response.Content.ReadAsStringAsync();
                var loginData = JsonSerializer.Deserialize<LoginResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginData == null || string.IsNullOrEmpty(loginData.Token))
                {
                    ErrorMessage = "Không nhận được token từ server!";
                    return Page();
                }

                // Lưu session
                HttpContext.Session.SetString("Token", loginData.Token);
                HttpContext.Session.SetString("UserName", Input.Username);
                HttpContext.Session.SetString("Roles", string.Join(",", loginData.Roles));

                _logger.LogInformation("User {User} logged in successfully", Input.Username);

                // Điều hướng theo role
                if (loginData.Roles.Contains("Admin"))
                    return RedirectToPage("/Dashboard/Admin");
                else if (loginData.Roles.Contains("Manager"))
                    return RedirectToPage("/Dashboard/Manager");
                else
                    return RedirectToPage("/Dashboard/Employee");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                ErrorMessage = "Không thể kết nối tới máy chủ!";
                return Page();
            }
        }
    }
}

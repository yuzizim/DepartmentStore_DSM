using DepartmentStore.Client.Models.Auth;
using DepartmentStore.Utilities.DTOs.Auth;
using System.Net.Http.Json;

namespace DepartmentStore.Client.Services
{
    public class AuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _context;

        public AuthService(IHttpClientFactory clientFactory, IHttpContextAccessor context)
        {
            _clientFactory = clientFactory;
            _context = context;
        }

        public async Task<bool> LoginAsync(LoginRequest model)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.PostAsJsonAsync("auth/login", model);

            if (!response.IsSuccessStatusCode)
                return false;

            var data = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (data == null || string.IsNullOrEmpty(data.Token))
                return false;

            _context.HttpContext?.Session.SetString("AccessToken", data.Token);
            _context.HttpContext?.Session.SetString("UserRole", data.Role ?? "");
            _context.HttpContext?.Session.SetString("Username", data.Username ?? "");

            return true;
        }

        public void Logout()
        {
            _context.HttpContext?.Session.Clear();
        }
    }
}

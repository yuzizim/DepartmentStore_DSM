using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace DepartmentStore.Client.Services
{
    public class ApiClientService
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _accessor;

        public ApiClientService(HttpClient client, IHttpContextAccessor accessor)
        {
            _client = client;
            _accessor = accessor;
            _client.BaseAddress = new Uri("https://localhost:7153/api/"); // CẬP NHẬT ĐÚNG PORT
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var token = _accessor.HttpContext?.Session.GetString("Token");

            // GHI LOG ĐỂ DEBUG
            Console.WriteLine($"[ApiClient] Calling: {endpoint}");
            Console.WriteLine($"[ApiClient] Token: {(string.IsNullOrEmpty(token) ? "MISSING" : "OK")}");

            // XÓA HEADER CŨ
            _client.DefaultRequestHeaders.Authorization = null;

            // GẮN TOKEN
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _client.GetAsync(endpoint);

            // IN RA NỘI DUNG ĐỂ DEBUG
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[ApiClient] Status: {response.StatusCode}");
            Console.WriteLine($"[ApiClient] Response (first 300 chars): {content.Substring(0, Math.Min(300, content.Length))}");

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _accessor.HttpContext?.Session.Clear();
                    throw new UnauthorizedAccessException("Phiên hết hạn");
                }
                response.EnsureSuccessStatusCode();
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
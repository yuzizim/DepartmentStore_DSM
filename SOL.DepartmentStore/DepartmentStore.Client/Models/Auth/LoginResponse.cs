namespace DepartmentStore.Client.Models.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public string? Role { get; set; }
        public string? FullName { get; set; }
    }
}

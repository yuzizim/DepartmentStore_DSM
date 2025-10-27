using DepartmentStore.Utilities.DTOs.Auth;

namespace DepartmentStore.Service.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task RegisterAsync(RegisterRequestDto request);
        Task LogoutAsync(Guid userId);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}

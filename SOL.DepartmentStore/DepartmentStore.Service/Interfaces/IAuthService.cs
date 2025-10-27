using DepartmentStore.Utilities.DTOs.Auth;

namespace DepartmentStore.Service.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task LogoutAsync(Guid userId);
        Task RegisterAsync(RegisterRequestDto request);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
    }
}

using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs;
using DepartmentStore.Utilities.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DepartmentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            await _authService.RegisterAsync(request);
            return StatusCode(201, "User registered successfully.");
        }

        // src/DepartmentStore.API/Controllers/AuthController.cs

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _authService.LogoutAsync(userId);
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _authService.GetAllUsersAsync();
            return Ok(result);
        }
    }
}

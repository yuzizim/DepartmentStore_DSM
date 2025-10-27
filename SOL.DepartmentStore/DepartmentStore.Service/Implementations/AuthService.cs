using AutoMapper;
using DepartmentStore.DataAccess;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Entities;
using DepartmentStore.Service.Interfaces;
using DepartmentStore.Utilities.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DepartmentStore.Service.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            AppDbContext context,
            IMapper mapper,
            IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _mapper = mapper;
            _config = config;
        }

        // ========== LOGIN ==========
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new UnauthorizedAccessException("Invalid username or password.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);

            // Tạo refresh token và lưu vào DB
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"), // 64 ký tự
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:DurationMinutes"])),
                Roles = roles,
                RefreshToken = refreshToken.Token
            };
        }

        // ========== REFRESH TOKEN ==========
        public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == refreshToken);

            if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.Expires < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");

            var user = tokenEntity.User!;
            var roles = await _userManager.GetRolesAsync(user);

            // Revoke old token
            tokenEntity.IsRevoked = true;

            // Create new token pair
            var newAccessToken = GenerateJwtToken(user, roles);
            var newRefreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = newAccessToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:DurationMinutes"])),
                Roles = roles,
                RefreshToken = newRefreshToken.Token
            };
        }

        // ========== LOGOUT ==========
        public async Task LogoutAsync(Guid userId)
        {
            var tokens = _context.RefreshTokens.Where(t => t.UserId == userId && !t.IsRevoked);
            await tokens.ForEachAsync(t => t.IsRevoked = true);
            await _context.SaveChangesAsync();
        }

        // ========== REGISTER ==========
        public async Task RegisterAsync(RegisterRequestDto request)
        {
            var user = new AppUser
            {
                UserName = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync(request.Role))
                throw new Exception($"Role '{request.Role}' does not exist.");

            await _userManager.AddToRoleAsync(user, request.Role);
        }

        // ========== GET ALL USERS ==========
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName!,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = string.Join(", ", roles)
                });
            }

            return userDtos;
        }

        // ========== PRIVATE HELPERS ==========
        private string GenerateJwtToken(AppUser user, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["JwtSettings:DurationMinutes"])),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

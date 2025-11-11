using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.DataAccess.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(Employee))]
        public Guid? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = null;
        public ICollection<Order> Orders { get; set; } = null;
    }
}
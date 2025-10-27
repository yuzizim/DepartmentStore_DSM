using DepartmentStore.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace DepartmentStore.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Guid? EmployeeId { get; set; }

        // Quan hệ 1-n với RefreshToken
        public ICollection<RefreshToken>? RefreshTokens { get; set; }

        // Quan hệ 1-n (nếu user cũng là Customer)
        public ICollection<Order>? Orders { get; set; }
    }
}

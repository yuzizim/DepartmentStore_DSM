using DepartmentStore.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DepartmentStore.DataAccess.Entities
{
    public class RefreshToken : BaseEntity
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        public DateTime Expires { get; set; }

        public bool IsRevoked { get; set; } = false;

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public AppUser? User { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;
    }
}

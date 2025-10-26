using DepartmentStore.DataAccess.Entities;
using System;

namespace DepartmentStore.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;

        public Guid UserId { get; set; }
        public AppUser? User { get; set; }
    }
}

﻿using Microsoft.AspNetCore.Identity;
using System;

namespace DepartmentStore.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

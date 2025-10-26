using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DepartmentStore.Entities
{
    public class Customer : BaseEntity
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}

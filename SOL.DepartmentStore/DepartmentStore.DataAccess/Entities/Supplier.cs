using DepartmentStore.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;

namespace DepartmentStore.Entities
{
    public class Supplier : BaseEntity
    {
        [Required, MaxLength(150)]
        public string Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}

using DepartmentStore.DataAccess.Entities;
using System.ComponentModel.DataAnnotations;

namespace DepartmentStore.Entities
{
    public class Category : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public string? Description { get; set; }

        public ICollection<Product>? Products { get; set; }
    }
}

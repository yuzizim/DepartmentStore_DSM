using System.ComponentModel.DataAnnotations;

namespace DepartmentStore.DataAccess.Entities
{
    public class Category : BaseEntity
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Product> Products { get; set; } = null;
    }
}
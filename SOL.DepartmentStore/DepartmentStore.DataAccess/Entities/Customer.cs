using System.ComponentModel.DataAnnotations;

namespace DepartmentStore.DataAccess.Entities
{
    public class Customer : BaseEntity
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        public ICollection<Order> Orders { get; set; } =  null;
    }
}
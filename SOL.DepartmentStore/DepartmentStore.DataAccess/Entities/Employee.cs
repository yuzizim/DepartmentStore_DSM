using System.ComponentModel.DataAnnotations;

namespace DepartmentStore.DataAccess.Entities
{
    public class Employee : BaseEntity
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Position { get; set; }

        public decimal Salary { get; set; }
    }
}
using DepartmentStore.DataAccess.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace DepartmentStore.Entities
{
    public class Employee : BaseEntity
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public string? Position { get; set; }
        public decimal Salary { get; set; }
    }
}

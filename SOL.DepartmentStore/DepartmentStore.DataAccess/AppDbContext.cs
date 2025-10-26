using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DepartmentStore.Entities;
using DepartmentStore.DataAccess.Entities;

namespace DepartmentStore.DataAccess
{
    // Dùng IdentityDbContext để hỗ trợ AppUser, AppRole
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ===== DbSet cho toàn bộ Entities =====
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Category - Product (1-n)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Supplier - Product (1-n)
            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Product - Inventory (1-1)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Order - OrderDetail (1-n)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Product - OrderDetail (1-n)
            modelBuilder.Entity<Product>()
                .HasMany<OrderDetail>()
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Order - Payment (1-1)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== User - RefreshToken (1-n)
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Decimal precision
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(od => od.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Employee>().Property(e => e.Salary).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);

            // ===== Seed Data mẫu =====
            var catId = Guid.NewGuid();
            var suppId = Guid.NewGuid();
            var prodId = Guid.NewGuid();

            modelBuilder.Entity<Category>().HasData(new Category
            {
                Id = catId,
                Name = "General",
                Description = "Default category",
                CreatedAt = DateTime.UtcNow
            });

            modelBuilder.Entity<Supplier>().HasData(new Supplier
            {
                Id = suppId,
                Name = "Default Supplier",
                Email = "supplier@example.com",
                Phone = "0123456789",
                CreatedAt = DateTime.UtcNow
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = prodId,
                Name = "Sample Item",
                Description = "Demo Product",
                Price = 9.99m,
                CategoryId = catId,
                SupplierId = suppId,
                CreatedAt = DateTime.UtcNow
            });

            modelBuilder.Entity<Inventory>().HasData(new Inventory
            {
                Id = Guid.NewGuid(),
                ProductId = prodId,
                QuantityOnHand = 150,
                LastRestockDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });

            // ===== Seed Roles mẫu =====
            var adminRoleId = Guid.NewGuid();
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = adminRoleId,
                Name = "ADMIN",
                NormalizedName = "ADMIN",
                Description = "System Administrator"
            });
        }
    }
}

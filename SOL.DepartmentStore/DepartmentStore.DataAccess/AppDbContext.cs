using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace DepartmentStore.DataAccess
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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

            // ===== Relationships =====
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Product>()
                .HasMany<OrderDetail>()
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Precision =====
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(od => od.UnitPrice).HasPrecision(18, 2);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Employee>().Property(e => e.Salary).HasPrecision(18, 2);
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);

            // ===== Seed Static Data =====
            var catId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var suppId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var prodId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var now = DateTime.Parse("2025-01-01T00:00:00Z");

            modelBuilder.Entity<Category>().HasData(new Category
            {
                Id = catId,
                Name = "General",
                Description = "Default category",
                CreatedAt = now
            });

            modelBuilder.Entity<Supplier>().HasData(new Supplier
            {
                Id = suppId,
                Name = "Default Supplier",
                Email = "supplier@example.com",
                Phone = "0123456789",
                CreatedAt = now
            });

            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = prodId,
                Name = "Sample Item",
                Description = "Demo Product",
                Price = 9.99m,
                CategoryId = catId,
                SupplierId = suppId,
                CreatedAt = now
            });

            modelBuilder.Entity<Inventory>().HasData(new Inventory
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                ProductId = prodId,
                QuantityOnHand = 150,
                LastRestockDate = now,
                CreatedAt = now
            });

            // ===== Static Seed Role =====
            var adminRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "System Administrator",
                CreatedAt = now
            });
        }
    }
}

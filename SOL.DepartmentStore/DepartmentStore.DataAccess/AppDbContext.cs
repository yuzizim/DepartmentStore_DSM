// src/DepartmentStore.DataAccess/AppDbContext.cs

using DepartmentStore.DataAccess.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace DepartmentStore.DataAccess
{
    /// <summary>
    /// Full AppDbContext with Identity + Soft Delete + Relationships + Seed Data
    /// </summary>
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // === DbSets ===
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

            // === 1. ÁP DỤNG TẤT CẢ CONFIG TỪ ASSEMBLY ===
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // === 2. GLOBAL QUERY FILTER: SOFT DELETE ===
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(BuildSoftDeleteFilter(entityType.ClrType));
                }
            }

            // === 3. RELATIONSHIPS ===
            ConfigureRelationships(modelBuilder);

            // === 4. PRECISION (DECIMAL) ===
            ConfigurePrecision(modelBuilder);

            // === 5. SEED DATA ===
            SeedData(modelBuilder);
        }

        // === HELPER: Soft Delete Filter ===
        private static LambdaExpression BuildSoftDeleteFilter(Type type)
        {
            var parameter = Expression.Parameter(type, "e");
            var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
            var condition = Expression.Equal(property, Expression.Constant(false));
            return Expression.Lambda(condition, parameter);
        }

        // === HELPER: Relationships ===
        private static void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Category → Product
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Supplier → Product
            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product → Inventory (1-1)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Inventory)
                .WithOne(i => i.Product)
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order → OrderDetail
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // === OrderDetail → Product (FIXED) ===
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)  // ← THÊM DÒNG NÀY
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order → Payment (1-1)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser → RefreshToken
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // AppUser → Employee (optional)
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Employee)
                .WithMany()
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        // === HELPER: Decimal Precision ===
        private static void ConfigurePrecision(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);
        }

        // === HELPER: Seed Data ===
        private static void SeedData(ModelBuilder modelBuilder)
        {
            var now = DateTime.Parse("2025-01-01T00:00:00Z");

            var catId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var suppId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var prodId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var invId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var adminRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");

            // Category
            modelBuilder.Entity<Category>().HasData(new Category
            {
                Id = catId,
                Name = "General",
                Description = "Default category for uncategorized products",
                CreatedAt = now
            });

            // Supplier
            modelBuilder.Entity<Supplier>().HasData(new Supplier
            {
                Id = suppId,
                Name = "Default Supplier",
                Email = "supplier@example.com",
                Phone = "0123456789",
                Address = "123 Default St, Hanoi, Vietnam",
                CreatedAt = now
            });

            // Product
            modelBuilder.Entity<Product>().HasData(new Product
            {
                Id = prodId,
                Name = "Sample Product",
                Description = "This is a demo product for testing",
                Price = 99.99m,
                CategoryId = catId,
                SupplierId = suppId,
                CreatedAt = now
            });

            // Inventory
            modelBuilder.Entity<Inventory>().HasData(new Inventory
            {
                Id = invId,
                ProductId = prodId,
                QuantityOnHand = 150,
                ReorderLevel = 10,
                LastRestockDate = now,
                CreatedAt = now
            });

            // Admin Role
            modelBuilder.Entity<AppRole>().HasData(new AppRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                Description = "System Administrator with full access",
                CreatedAt = now
            });
        }
    }
}
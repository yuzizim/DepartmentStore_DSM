using System;
using System.Linq;
using System.Threading.Tasks;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DepartmentStore.DataAccess
{
    public static class SeedDb
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

            // Ensure database is created & migrated
            await context.Database.MigrateAsync();

            // ====================== ROLES ======================
            string[] roles = new[] { "ADMIN", "USER" };
            foreach (var roleName in roles)
            {
                if (!await roleManager.Roles.AnyAsync(r => r.Name == roleName))
                {
                    await roleManager.CreateAsync(new AppRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        Description = $"{roleName} role"
                    });
                }
            }

            // ====================== ADMIN USER ======================
            var adminEmail = "admin@dsm.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "ADMIN");
                }
            }

            // ====================== DEFAULT DATA ======================
            if (!await context.Categories.AnyAsync())
            {
                var catId = Guid.NewGuid();
                var supId = Guid.NewGuid();
                var prodId = Guid.NewGuid();

                var category = new Category
                {
                    Id = catId,
                    Name = "General",
                    Description = "Default category",
                    CreatedAt = DateTime.UtcNow
                };

                var supplier = new Supplier
                {
                    Id = supId,
                    Name = "Default Supplier",
                    Email = "supplier@example.com",
                    Phone = "0123456789",
                    CreatedAt = DateTime.UtcNow
                };

                var product = new Product
                {
                    Id = prodId,
                    Name = "Sample Product",
                    Description = "This is a demo item.",
                    Price = 9.99m,
                    CategoryId = catId,
                    SupplierId = supId,
                    CreatedAt = DateTime.UtcNow
                };

                var inventory = new Inventory
                {
                    Id = Guid.NewGuid(),
                    ProductId = prodId,
                    QuantityOnHand = 100,
                    LastRestockDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await context.Categories.AddAsync(category);
                await context.Suppliers.AddAsync(supplier);
                await context.Products.AddAsync(product);
                await context.Inventories.AddAsync(inventory);
                await context.SaveChangesAsync();
            }
        }
    }
}

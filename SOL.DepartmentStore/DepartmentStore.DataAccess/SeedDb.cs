using DepartmentStore.DataAccess.Entities;
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

            await context.Database.MigrateAsync();

            // 1️⃣ Seed Roles
            string[] roles = { "Admin", "Manager", "SalesEmployee", "InventoryEmployee" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper(),
                        Description = $"{role} role created on seed."
                    });
                }
            }

            // 2️⃣ Seed Users
            await SeedUserAsync(userManager, "admin@dsm.com", "Admin", "System Administrator", new[] { "Admin" });
            await SeedUserAsync(userManager, "manager@dsm.com", "storemanager", "Store Manager", new[] { "Manager" });
            await SeedUserAsync(userManager, "sales@dsm.com", "salesuser", "Sales Employee", new[] { "SalesEmployee" });
            await SeedUserAsync(userManager, "inventory@dsm.com", "inventoryuser", "Inventory Employee", new[] { "InventoryEmployee" });

            // 3️⃣ Seed Sample Data (if empty)
            if (!context.Categories.Any())
            {
                var cat = new Category { Id = Guid.NewGuid(), Name = "General", Description = "General category", CreatedAt = DateTime.UtcNow };
                context.Categories.Add(cat);

                var sup = new Supplier
                {
                    Id = Guid.NewGuid(),
                    Name = "Default Supplier",
                    Email = "supplier@dsm.com",
                    Phone = "0123456789",
                    Address = "Hanoi, Vietnam",
                    CreatedAt = DateTime.UtcNow
                };
                context.Suppliers.Add(sup);

                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Sample Product",
                    Description = "Seeded product for testing",
                    Price = 100000,
                    CategoryId = cat.Id,
                    SupplierId = sup.Id,
                    CreatedAt = DateTime.UtcNow
                };
                context.Products.Add(product);

                var inv = new Inventory
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    QuantityOnHand = 200,
                    ReorderLevel = 20,
                    LastRestockDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };
                context.Inventories.Add(inv);

                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUserAsync(
            UserManager<AppUser> userManager,
            string email,
            string username,
            string fullname,
            string[] roles)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var user = new AppUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = fullname,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, "Test@123");
                if (result.Succeeded)
                {
                    foreach (var role in roles)
                        await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}

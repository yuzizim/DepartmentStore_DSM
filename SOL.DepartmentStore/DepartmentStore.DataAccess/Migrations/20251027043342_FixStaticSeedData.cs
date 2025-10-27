using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DepartmentStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixStaticSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("4e0f4f33-bd01-47d8-a5be-767414d5ceb6"));

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: new Guid("db42637c-fb1c-4edb-81aa-4de205da032d"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("0d8a6231-6558-4332-99cf-94ac0d18546d"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("f1c84c00-8395-4d86-9434-2f06a48d3089"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("fba05ca3-28d2-4b97-8714-2fda7d3c71b5"));

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "AspNetUsers",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "Name", "NormalizedName" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), null, new DateTime(2025, 1, 1, 7, 0, 0, 0, DateTimeKind.Local), "System Administrator", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 7, 0, 0, 0, DateTimeKind.Local), "Default category", false, "General", null });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "IsDeleted", "Name", "Phone", "UpdatedAt" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), null, new DateTime(2025, 1, 1, 7, 0, 0, 0, DateTimeKind.Local), "supplier@example.com", false, "Default Supplier", "0123456789", null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsDeleted", "Name", "Price", "SupplierId", "UpdatedAt" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 7, 0, 0, 0, DateTimeKind.Local), "Demo Product", false, "Sample Item", 9.99m, new Guid("22222222-2222-2222-2222-222222222222"), null });

            migrationBuilder.InsertData(
                table: "Inventories",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "LastRestockDate", "ProductId", "QuantityOnHand", "ReorderLevel", "UpdatedAt" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), new DateTime(2025, 1, 1, 7, 0, 0, 0, DateTimeKind.Local), false, new DateTime(2025, 1, 1, 7, 0, 0, 0, DateTimeKind.Local), new Guid("33333333-3333-3333-3333-333333333333"), 150, 10, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Inventories",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedAt", "Description", "Name", "NormalizedName" },
                values: new object[] { new Guid("4e0f4f33-bd01-47d8-a5be-767414d5ceb6"), null, new DateTime(2025, 10, 26, 13, 38, 13, 28, DateTimeKind.Utc).AddTicks(6269), "System Administrator", "ADMIN", "ADMIN" });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[] { new Guid("f1c84c00-8395-4d86-9434-2f06a48d3089"), new DateTime(2025, 10, 26, 13, 38, 13, 26, DateTimeKind.Utc).AddTicks(9292), "Default category", false, "General", null });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "IsDeleted", "Name", "Phone", "UpdatedAt" },
                values: new object[] { new Guid("fba05ca3-28d2-4b97-8714-2fda7d3c71b5"), null, new DateTime(2025, 10, 26, 13, 38, 13, 28, DateTimeKind.Utc).AddTicks(422), "supplier@example.com", false, "Default Supplier", "0123456789", null });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "IsDeleted", "Name", "Price", "SupplierId", "UpdatedAt" },
                values: new object[] { new Guid("0d8a6231-6558-4332-99cf-94ac0d18546d"), new Guid("f1c84c00-8395-4d86-9434-2f06a48d3089"), new DateTime(2025, 10, 26, 13, 38, 13, 28, DateTimeKind.Utc).AddTicks(3079), "Demo Product", false, "Sample Item", 9.99m, new Guid("fba05ca3-28d2-4b97-8714-2fda7d3c71b5"), null });

            migrationBuilder.InsertData(
                table: "Inventories",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "LastRestockDate", "ProductId", "QuantityOnHand", "ReorderLevel", "UpdatedAt" },
                values: new object[] { new Guid("db42637c-fb1c-4edb-81aa-4de205da032d"), new DateTime(2025, 10, 26, 13, 38, 13, 28, DateTimeKind.Utc).AddTicks(5145), false, new DateTime(2025, 10, 26, 13, 38, 13, 28, DateTimeKind.Utc).AddTicks(4951), new Guid("0d8a6231-6558-4332-99cf-94ac0d18546d"), 150, 10, null });
        }
    }
}

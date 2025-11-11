using DepartmentStore.DataAccess.Entities;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace DepartmentStore.API.OData
{
    public static class EdmModelBuilder
    {
        public static IEdmModel GetEdmModel()
        {
            var builder = new ODataConventionModelBuilder();

            // === EXPOSE ENTITIES ===
            builder.EntitySet<Category>("Categories");
            builder.EntitySet<Supplier>("Suppliers");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Inventory>("Inventories");
            builder.EntitySet<Customer>("Customers");
            builder.EntitySet<Order>("Orders");
            builder.EntitySet<OrderDetail>("OrderDetails");
            builder.EntitySet<Payment>("Payments");

            // === FUNCTIONS / ACTIONS (TÙƠNG LAI) ===
            // var lowStock = builder.EntityType<Inventory>().Collection.Function("LowStock");
            // lowStock.ReturnsCollectionFromEntitySet<Inventory>("Inventories");

            return builder.GetEdmModel();
        }
    }
}
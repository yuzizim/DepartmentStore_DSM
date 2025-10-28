using AutoMapper;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Entities;
using DepartmentStore.Utilities.DTOs;
using DepartmentStore.Utilities.DTOs.Auth;

namespace DepartmentStore.Utilities.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.QuantityOnHand, opt => opt.MapFrom(src => src.Inventory != null ? src.Inventory.QuantityOnHand : 0))
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(d => d.SupplierName, opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.Name : null));

            CreateMap<CreateProductDto, Product>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore());

            CreateMap<UpdateProductDto, Product>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Category
            CreateMap<Category, CategoryDto>();
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Supplier
            CreateMap<Supplier, SupplierDto>();
            CreateMap<CreateSupplierDto, Supplier>();
            CreateMap<UpdateSupplierDto, Supplier>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Inventory
            CreateMap<Inventory, InventoryDto>();

            // ===== Order Mappings =====
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<OrderDetail, OrderDetailDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<CreateOrderDto, Order>()
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.OrderDetails, opt => opt.Ignore());

            CreateMap<CreateOrderDetailDto, OrderDetail>()
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore());

            // ===== Payment Mappings =====
            CreateMap<Payment, PaymentDto>();


            // ===== User Mapping =====
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // ===== RefreshToken Mapping =====
            CreateMap<RefreshToken, RefreshTokenDto>();
        }
    }
}

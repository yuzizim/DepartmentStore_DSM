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
            // ===== Product Mapping =====
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();

            // ===== User Mapping =====
            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // ===== RefreshToken Mapping =====
            CreateMap<RefreshToken, RefreshTokenDto>();
        }
    }
}

using AutoMapper;
using DepartmentStore.DataAccess.Entities;
using DepartmentStore.Utilities.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DepartmentStore.Utilities.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<CreateProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
        }
    }
}

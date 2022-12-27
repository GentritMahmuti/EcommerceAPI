using AutoMapper;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Helpers
{
    public class AutoMapperConfigurations : Profile
    {
        public AutoMapperConfigurations()
        {
            CreateMap<Product, ProductCreateDto>().ReverseMap();
            CreateMap<Category, CategoryCreateDto>().ReverseMap();
        }

    }
}

using AutoMapper;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.OrderDetails;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Helpers
{
    public class AutoMapperConfigurations : Profile
    {
        public AutoMapperConfigurations()
        {
            CreateMap<Product, ProductCreateDto>().ReverseMap();
            CreateMap<Product, ProductCreateElasticDto>().ReverseMap();
            CreateMap<Category, CategoryCreateDto>().ReverseMap();
            CreateMap<OrderDetails, OrderDetailsCreateDto>().ReverseMap();
            CreateMap<Review, ReviewCreateDto>().ReverseMap();
        }
    }
}

using AutoMapper;
using EcommerceAPI.Models.DTOs.Category;
using EcommerceAPI.Models.DTOs.Chat;
using EcommerceAPI.Models.DTOs.Order;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Helpers
{
    public class AutoMapperConfigurations : Profile
    {
        public AutoMapperConfigurations()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, ProductCreateDto>().ReverseMap();

            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Category, CategoryCreateDto>().ReverseMap();

            CreateMap<OrderData, OrderDataDto>().ReverseMap();
            CreateMap<OrderData, OrderDataCreateDto>().ReverseMap();

            CreateMap<ProductOrderData, ProductOrderDataDto>().ReverseMap();
            CreateMap<ProductOrderData, ProductOrderDataCreateDto>().ReverseMap();

            CreateMap <Promotion, PromotionDto>().ReverseMap();
            CreateMap<Promotion, PromotionDetailsDto>().ReverseMap();

            CreateMap<Review, ReviewCreateDto>().ReverseMap();


            CreateMap<ChatMessage, ChatDTO>().ReverseMap();

            CreateMap<Review, ReviewDetailsDto>().ReverseMap();

        }
    }
}

using AutoMapper;
using Domain.Entities;
using Services.DTOs.Category;
using Services.DTOs.Chat;
using Services.DTOs.Order;
using Services.DTOs.Product;
using Services.DTOs.Promotion;
using Services.DTOs.Review;

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



            CreateMap <Promotion, PromotionDto>().ReverseMap();
            CreateMap<Promotion, PromotionDetailsDto>().ReverseMap();

            CreateMap<Review, ReviewCreateDto>().ReverseMap();


            CreateMap<ChatMessage, ChatDTO>().ReverseMap();

            CreateMap<Review, ReviewDetailsDto>().ReverseMap();

        }
    }
}

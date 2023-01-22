using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{
    public interface IPromotionService
    {
        Task CreatePromotion(PromotionCreateDto promotionToCreate);
        Task DeletePromotion(int id);
        Task<List<Promotion>> GetAllPromotions();
        Task<Promotion> GetPromotion(int id);
        Task UpdatePromotion(Promotion promotionToUpdate);
    }
}

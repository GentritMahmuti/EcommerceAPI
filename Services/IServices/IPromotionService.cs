

using Services.DTOs.Promotion;

namespace Services.Services.IServices
{
    public interface IPromotionService
    {
        Task CreatePromotion(PromotionDto promotionToCreate);
        Task DeletePromotion(int id);
        Task<List<PromotionDetailsDto>> GetAllPromotions();
        Task<PromotionDetailsDto> GetPromotionDetails(int id);
        Task UpdatePromotion(int id, PromotionDto promotionToUpdate);
    }
}

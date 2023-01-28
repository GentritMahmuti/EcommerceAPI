using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceAPI.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PromotionService> _logger;


        public PromotionService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ILogger<PromotionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task<PromotionDetailsDto> GetPromotionDetails(int id)
        {
            var promotion = await GetPromotion(id); 
            var promotionDetails = _mapper.Map<PromotionDetailsDto>(promotion);

            return promotionDetails;
        }
        
        public async Task<List<PromotionDetailsDto>> GetAllPromotions()
        {
            var promotions = _unitOfWork.Repository<Promotion>().GetAll().ToList();
            var promotionsDetails = _mapper.Map<List<Promotion>, List<PromotionDetailsDto>>(promotions);
            return promotionsDetails;
        }

        
        public async Task CreatePromotion(PromotionDto promotionToCreate)
        {
            var promotion = _mapper.Map<Promotion>(promotionToCreate);

            _unitOfWork.Repository<Promotion>().Create(promotion);
            _unitOfWork.Complete();
            _logger.LogInformation("Created promotion code successfully!");

        }

        
        public async Task UpdatePromotion(int id, PromotionDto promotionToUpdate)
        {
            var promotion = await GetPromotion(id);
            if (promotion == null)
            {
                throw new NullReferenceException("The promotion you're trying to update doesn't exist!");
            }
            promotion.Name = promotionToUpdate.Name;
            promotion.StartDate = promotionToUpdate.StartDate;
            promotion.EndDate = promotionToUpdate.EndDate;
            promotion.DiscountAmount = promotionToUpdate.DiscountAmount;

            _unitOfWork.Repository<Promotion>().Update(promotion);

            _unitOfWork.Complete();
        }

       
        public async Task DeletePromotion(int id)
        {
            var promotion = await GetPromotion(id);
            if (promotion == null)
            {
                throw new NullReferenceException("The promotion you're trying to delete doesn't exist.");
            }
            _unitOfWork.Repository<Promotion>().Delete(promotion);
            _unitOfWork.Complete();
            _logger.LogInformation("Deleted promotion successfully!");

        }
        private async Task<Promotion> GetPromotion(int id)
        {
            Expression<Func<Promotion, bool>> expression = x => x.Id == id;
            var promotion = await _unitOfWork.Repository<Promotion>().GetById(expression).FirstOrDefaultAsync();
            return promotion;
        }
    }
}

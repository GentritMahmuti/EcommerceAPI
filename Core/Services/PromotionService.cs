using AutoMapper;
using Persistence.UnitOfWork.IUnitOfWork;
using Domain.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.DTOs.Promotion;
using Services.Services.IServices;
using System.Linq.Expressions;

namespace Services.Services
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
            if (promotion == null)
            {
                throw new NullReferenceException("The promotion you're trying to get doesn't exist.");
            }
            var promotionDetails = _mapper.Map<PromotionDetailsDto>(promotion);

            return promotionDetails;
        }
        
        public async Task<List<PromotionDetailsDto>> GetAllPromotions()
        {
            var promotions = await _unitOfWork.Repository<Promotion>().GetAll().ToListAsync();
            var promotionsDetails = _mapper.Map<List<Promotion>, List<PromotionDetailsDto>>(promotions);
            return promotionsDetails;
        }

        
        public async Task CreatePromotion(PromotionDto promotionToCreate)
        {
            var promotion = _mapper.Map<Promotion>(promotionToCreate);

            _unitOfWork.Repository<Promotion>().Create(promotion);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"{nameof(PromotionService)} - Created promotion code successfully!");

        }


        /// <summary>
        /// Updates a specific promotion.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatePromotion"></param>
        /// <returns></returns>
        public async Task UpdatePromotion(int id, PromotionDto promotionToUpdate)
        {
            var promotion = await GetPromotion(id);
            if (promotion == null)
            {
                throw new NullReferenceException("The promotion you're trying to update doesn't exist.");
            }

            promotion.Name = promotionToUpdate.Name;
            promotion.StartDate = promotionToUpdate.StartDate;
            promotion.EndDate = promotionToUpdate.EndDate;
            promotion.DiscountAmount = promotionToUpdate.DiscountAmount;

            _unitOfWork.Repository<Promotion>().Update(promotion);

            await _unitOfWork.CompleteAsync();
        }

       

        /// <summary>
        /// Deletes a specific promotion.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public async Task DeletePromotion(int id)
        {
            var promotion = await GetPromotion(id);
            if (promotion == null)
            {
                throw new NullReferenceException("The promotion you're trying to delete doesn't exist.");
            }
            _unitOfWork.Repository<Promotion>().Delete(promotion);
            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"{nameof(PromotionService)} - Deleted promotion successfully!");

        }

        private async Task<Promotion> GetPromotion(int id)
        {
            var promotion = await _unitOfWork.Repository<Promotion>().GetById(x => x.Id == id).FirstOrDefaultAsync();
            return promotion;
        }
    }
}

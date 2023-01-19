using EcommerceAPI.Models.DTOs.CoverType;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{ 
    public interface ICoverTypeService
    {
        Task CreateCover(CoverTypeCreateDto coverToCreate);
        Task<List<CoverType>> GetAllCovers();
        Task<CoverType> GetCover(int id);
        Task UpdateCover(int id, CoverTypeDto coverToUpdate);
        Task DeleteCover(int id);
    }
}

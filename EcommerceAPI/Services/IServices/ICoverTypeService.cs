using EcommerceAPI.Models.DTOs.CoverType;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{ 
    public interface ICoverTypeService
    {
        Task CreateCover(CoverTypeCreateDTO coverToCreate);
        Task<List<CoverType>> GetAllCovers();
        Task<CoverType> GetCover(int id);
        Task UpdateCover(CoverTypeCreateDTO coverToUpdate);
        Task DeleteCover(int id);
    }
}

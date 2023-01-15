using EcommerceAPI.Models.DTOs.CoverType;
using EcommerceAPI.Models.Entities;

namespace EcommerceAPI.Services.IServices
{ 
    public interface ICoverTypeService
    {
        Task CreateCover(CoverTypeDTO coverToCreate);
        Task<List<CoverType>> GetAllCovers();
        Task<CoverType> GetCover(int id);
        Task UpdateCover(int id, CoverTypeDTO coverToUpdate);
        Task DeleteCover(int id);
    }
}

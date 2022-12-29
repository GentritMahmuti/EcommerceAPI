using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.CoverType;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceAPI.Services
{
    public class CoverTypeService : ICoverTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateCover(CoverTypeCreateDTO coverToCreate)
        {
            var cover = new CoverType
            {
                Name = coverToCreate.Name
            };

            _unitOfWork.Repository<CoverType>().Create(cover);

            _unitOfWork.Complete();
        }

        public async Task<List<CoverType>> GetAllCovers()
        {
            var covers = _unitOfWork.Repository<CoverType>().GetAll().ToList();

            return covers.ToList();
        }

        public async Task<CoverType> GetCover(int id)
        {
           
            var cover = await _unitOfWork.Repository<CoverType>().GetById(x => x.Id == id).FirstOrDefaultAsync();

            return cover;
        }

        public async Task UpdateCover(CoverTypeDTO coverToUpdate)
        {
            var cover = await GetCover(coverToUpdate.Id);

            cover.Name = coverToUpdate.Name;

            _unitOfWork.Repository<CoverType>().Update(cover);

            _unitOfWork.Complete();
        }

        public async Task DeleteCover(int id)
        {
            var cover = await GetCover(id);

            _unitOfWork.Repository<CoverType>().Delete(cover);

            _unitOfWork.Complete();
        }
    }
}


using AutoMapper;
using EcommerceAPI.Models.DTOs.DataOrder;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Services
{
    public class OrderDataService : IOrderDataService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderDataService> _logger;

        public OrderDataService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ILogger<OrderDataService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<OrderData> GetOrderData(int id)
        {
            var orderData = await _unitOfWork.Repository<OrderData>().GetById(x => x.Id == id).FirstOrDefaultAsync();

            return orderData;
        }

        public async Task<List<OrderData>> GetAllOrderData()
        {
            var orderData = _unitOfWork.Repository<OrderData>().GetAll();
            return orderData.ToList();
        }


        public async Task CreateOrderData(OrderDataCreateDto orderDataToCreate)
        {
            var orderData = _mapper.Map<OrderData>(orderDataToCreate);

            _unitOfWork.Repository<OrderData>().Create(orderData);
            _unitOfWork.Complete();
            _logger.LogInformation("Created orderData successfully!");

        }


        public async Task CreateAllOrderData(List<OrderDataCreateDto> orderDataToCreate)
        {
            var orderData = _mapper.Map<List<OrderDataCreateDto>, List<OrderData>>(orderDataToCreate);
            _unitOfWork.Repository<OrderData>().CreateRange(orderData);
            _unitOfWork.Complete();
            _logger.LogInformation("Created orderData successfully!");

        }

        public async Task DeleteOrderData(int id)
        {
            var orderData = await GetOrderData(id);
            if (orderData == null)
            {
                throw new NullReferenceException("The orderData you're trying to delete doen't exist.");
            }

            _unitOfWork.Repository<OrderData>().Delete(orderData);
            _unitOfWork.Complete();
            _logger.LogInformation("Deleted orderData successfully!");

        }

        public async Task UpdateOrderData(OrderData orderDataToUpdate)
        {
            var orderData = await GetOrderData(orderDataToUpdate.Id);
            if (orderData == null)
            {
                throw new NullReferenceException("The orderDetails you're trying to update doesn't exist!");
            }
            orderData.PhoheNumber = orderDataToUpdate.PhoheNumber;
            orderData.StreetAddress = orderDataToUpdate.StreetAddress;
            orderData.City = orderDataToUpdate.City;
            orderData.Country = orderDataToUpdate.Country;
            orderData.PostalCode = orderDataToUpdate.PostalCode;
            orderData.Name = orderDataToUpdate.Name;

            _unitOfWork.Repository<OrderData>().Update(orderData);

            _unitOfWork.Complete();
        }


    }
}

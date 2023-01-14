using AutoMapper;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Models.DTOs.OrderDetails;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EcommerceAPI.Services
{
    public class OrderDetailsService  : IOrderDetailsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderDetailsService> _logger;


        public OrderDetailsService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration, ILogger<OrderDetailsService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }


        public async Task<OrderDetails> GetOrderDetails(int id)
        {
            var orderDetails = await _unitOfWork.Repository<OrderDetails>().GetById(x => x.Id == id).FirstOrDefaultAsync();

            return orderDetails;
        }

        public async Task<List<OrderDetails>> GetAllOrderDetails()
        {
            var orderDetails = _unitOfWork.Repository<OrderDetails>().GetAll();
            return orderDetails.ToList();
        }


        public async Task CreateOrderDetails(OrderDetailsCreateDto orderDetailsToCreate)
        {
            var orderDetails = _mapper.Map<OrderDetails>(orderDetailsToCreate);

            _unitOfWork.Repository<OrderDetails>().Create(orderDetails);
            _unitOfWork.Complete();
            _logger.LogInformation("Created orderDetails successfully!");

        }


        public async Task CreateAllOrderDetails(List<OrderDetailsCreateDto> orderDetailssToCreate)
        {
            var orderDetails = _mapper.Map<List<OrderDetailsCreateDto>, List<OrderDetails>>(orderDetailssToCreate);
            _unitOfWork.Repository<OrderDetails>().CreateRange(orderDetails);
            _unitOfWork.Complete();
            _logger.LogInformation("Created orderDetails successfully!");

        }

        public async Task DeleteOrderDetails(int id)
        {
            var orderDetails = await GetOrderDetails(id);
            if (orderDetails == null)
            {
                throw new NullReferenceException("The orderDetails you're trying to delete doesn't exist.");
            }

            _unitOfWork.Repository<OrderDetails>().Delete(orderDetails);
            _unitOfWork.Complete();
            _logger.LogInformation("Deleted orderDetails successfully!");

        }

        public async Task UpdateOrderDetails(OrderDetails orderDetailsToUpdate)
        {
            var orderDetails = await GetOrderDetails(orderDetailsToUpdate.Id);
            if (orderDetails == null)
            {
                throw new NullReferenceException("The orderDetails you're trying to update doesn't exist!");
            }
            orderDetails.Id = orderDetailsToUpdate.Id;
            orderDetails.OrderData = orderDetailsToUpdate.OrderData;
            orderDetails.ProductId = orderDetailsToUpdate.ProductId;
            orderDetails.Count = orderDetailsToUpdate.Count;
            orderDetails.Price = orderDetailsToUpdate.Price;

            _unitOfWork.Repository<OrderDetails>().Update(orderDetails);

            _unitOfWork.Complete();
        }

       
    }
}

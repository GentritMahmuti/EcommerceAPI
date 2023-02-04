
using Persistence.UnitOfWork.IUnitOfWork;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Mail;
using Services.DTOs;

namespace EcommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerSupportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        public CustomerSupportController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        [HttpPost("inquiry")]
        public async Task<IActionResult> SubmitInquiry(InquiryModel inquiry)
        {
            // Validate incoming data
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Store the inquiry in database
            var newInquiry = new Inquiry
            {
                Email = inquiry.Email,
                Message = inquiry.Message,
                SubmittedAt = DateTime.UtcNow
            };
            _unitOfWork.Repository<Inquiry>().Create(newInquiry);
            _unitOfWork.Complete();

            // Send email notification to support agents
            await _emailSender.SendEmailAsync("elmedina.lahu@life.gjirafa.com","New Customer Inquiry", $"A new customer inquiry was submitted by {inquiry.Email}. Message: {inquiry.Message}");

            return Ok();
        }
    }
}

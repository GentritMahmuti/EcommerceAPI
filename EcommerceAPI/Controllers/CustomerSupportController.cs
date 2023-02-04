
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

        /// <summary>
        /// Submits a customer inquiry.
        /// </summary>
        /// <param name="inquiry">The inquiry data including email and message from the customer</param>
        /// <returns>An HTTP 200 OK response if the inquiry was successfully submitted</returns>
        /// <response code="200">The inquiry was successfully submitted</response>
        /// <response code="400">If the provided data is invalid</response>
        /// <tags>Inquiry</tags>
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

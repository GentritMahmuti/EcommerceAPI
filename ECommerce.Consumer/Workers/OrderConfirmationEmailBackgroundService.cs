using Microsoft.AspNetCore.Identity.UI.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using EcommerceAPI.Models.DTOs.Order;

namespace ECommerce.Consumer.Workers
{
    public class OrderConfirmationEmailBackgroundService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<OrderStatusEmailBackgroundService> _logger;


        public OrderConfirmationEmailBackgroundService(IEmailSender emailSender, ILogger<OrderStatusEmailBackgroundService> logger)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _emailSender = emailSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare("order-confirmations", exclusive: false, durable: true, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, args) =>
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var data = JsonConvert.DeserializeObject<OrderConfirmationDto>(message);

                _logger.LogInformation($"Data for the order with Id: '{data.OrderId}' is consumed successfully!");
                SendStatusEmail(data);
            };

            _channel.BasicConsume(queue: "order-confirmations",
                                  autoAck: true,
                                  consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100000, stoppingToken);
            }
        }

        private void SendStatusEmail(OrderConfirmationDto data)
        {
            var pathToFile = "Templates/orderConfirmation.html";

            string htmlBody = "";
            using (StreamReader streamReader = File.OpenText(pathToFile))
            {
                htmlBody = streamReader.ReadToEnd();
            }

            var contentData = new string[] { data.UserName, data.OrderDate.ToString(), data.Price.ToString(), data.OrderId};

            var content = string.Format(htmlBody, contentData);

            try
            {
                _logger.LogInformation("Sending 'Order Confirmation' email!");
                //_emailSender.SendEmailAsync(data.Email, "Order Status", content);
                _emailSender.SendEmailAsync("elmedina.lahu@life.gjirafa.com", "Order Confirmation", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email for order confirmation!");
            }
        }
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}

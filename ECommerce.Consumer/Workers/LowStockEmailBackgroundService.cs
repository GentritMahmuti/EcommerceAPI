using Microsoft.AspNetCore.Identity.UI.Services;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Ecommerce.Consumer.Entities;
using System.Text;
using Newtonsoft.Json;

namespace ECommerce.Consumer.Workers
{
    public class LowStockEmailBackgroundService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<LowStockEmailBackgroundService> _logger;


        public LowStockEmailBackgroundService(IEmailSender emailSender, ILogger<LowStockEmailBackgroundService> logger)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _emailSender = emailSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare("low-stock", exclusive: false, durable: true, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, args) => 
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var data = JsonConvert.DeserializeObject<LowStockDto>(message);

                _logger.LogInformation($"Data for the product with Id: '{data.ProductId}' with low stock is consumed successfully!");
                SendEmail(data);
            };

            _channel.BasicConsume(queue: "low-stock",
                                  autoAck: true,
                                  consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100000, stoppingToken);
            }
        }

        private async Task SendEmail(LowStockDto data)
        {
            var pathToFile = "Templates/lowStock.html";

            string htmlBody = "";
            using (StreamReader streamReader = File.OpenText(pathToFile))
            {
                htmlBody = streamReader.ReadToEnd();
            }

            var contentData = new string[] { data.ProductId.ToString(), data.CurrStock.ToString(), DateTime.Now.ToString()};

            var content = string.Format(htmlBody, contentData);

            try
            {
                _logger.LogInformation("Sending 'Low Stock' email!");
                await _emailSender.SendEmailAsync("jetonsllamniku@gmail.com", "Alert: Low Stock", content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email for low stock!");
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

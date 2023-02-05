using ECommerce.Consumer.Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Text;

namespace ECommerce.Consumer.Workers
{
    public class OrderStatusEmailBackgroundService : BackgroundService
    {
        private IConnection _connection;
        private IModel _channel;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<OrderStatusEmailBackgroundService> _logger;


        public OrderStatusEmailBackgroundService(IEmailSender emailSender, ILogger<OrderStatusEmailBackgroundService> logger)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _emailSender = emailSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel.QueueDeclare("orders", exclusive: false, durable: true, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, args) =>
            {
                if (stoppingToken.IsCancellationRequested)
                    return;

                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                var data = JsonConvert.DeserializeObject<OrderStatusDto>(message);

                _logger.LogInformation($"Data for the order with Id: '{data.OrderId}' is consumed successfully!");
                SendStatusEmail(data);
            };

            _channel.BasicConsume(queue: "orders",
                                  autoAck: true,
                                  consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(100000, stoppingToken);
            }
        }

        private async Task SendStatusEmail(OrderStatusDto data) 
        {
            var pathToFile = "Templates/orderStatus.html";

            string htmlBody = "";
            using (StreamReader streamReader = File.OpenText(pathToFile))
            {
                htmlBody = streamReader.ReadToEnd();
            }

            var contentData = new string[] { data.OrderId, data.Name, data.Status };

            var content = string.Format(htmlBody, contentData);

            try
            {
                _logger.LogInformation("Sending 'Order Status has changed' email!");
                //_emailSender.SendEmailAsync(data.Email, "Order Status", content);
                await _emailSender.SendEmailAsync("jetonsllamniku@gmail.com", "Order Status", content);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email for order status!");
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

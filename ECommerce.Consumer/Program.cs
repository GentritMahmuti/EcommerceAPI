using Ecommerce.Consumer.EmailSender;
using ECommerce.Consumer.Extensions;
using ECommerce.Consumer.Workers;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<OrderStatusEmailBackgroundService>();
builder.Services.AddHostedService<OrderConfirmationEmailBackgroundService>();


builder.Services.AddEmailSenders(builder.Configuration);

var smtpConfigurations = builder.Configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();
builder.Services.AddSingleton(smtpConfigurations);
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();


var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

using AutoMapper;
using EcommerceAPI.Data;
using EcommerceAPI.Data.UnitOfWork;
using EcommerceAPI.Helpers;
using EcommerceAPI.Helpers.EmailSender;
using EcommerceAPI.Models.DTOs.Product;
using EcommerceAPI.Models.DTOs.Review;
using EcommerceAPI.Models.Entities;
using EcommerceAPI.Services.IServices;
using EcommerceAPI.Services;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using Serilog;
using System.Security.Claims;
using System.Text;
using claims = System.Security.Claims;
using Stripe;
using EcommerceAPI;
using EcommerceAPI.Infrastructure;
using EcommerceAPI.Hubs;
using EcommerceAPI.Workers;
using FluentAssertions.Common;
using EcommerceAPI.Models.DTOs.Promotion;
using EcommerceAPI.Validators.EntityValidators;
using EcommerceAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
})
              .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
              {
                  options.Authority = "https://sso-sts.gjirafa.dev";
                  options.RequireHttpsMetadata = false;
                  options.Audience = "life_api";
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = "https://sso-sts.gjirafa.dev",
                      ValidAudience = "life_api",
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("21551351-fc9a-4d8e-8619-8c7e5acb6d47")),
                      ClockSkew = TimeSpan.Zero
                  };

                  options.Events = new JwtBearerEvents
                  {
                      OnTokenValidated = async context =>
                      {
                          context.HttpContext.User = context.Principal ?? new claims.ClaimsPrincipal();

                          var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                          var firstName = context.HttpContext.User.FindFirst(ClaimTypes.GivenName)?.Value;
                          var lastName = context.HttpContext.User.FindFirst(ClaimTypes.Surname)?.Value;
                          var email = context.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
                          var gender = context.HttpContext.User.FindFirst(ClaimTypes.Gender)?.Value;
                          var birthdate = context.HttpContext.User.FindFirst(ClaimTypes.DateOfBirth)?.Value;
                          var phone = context.HttpContext.User.FindFirst(ClaimTypes.MobilePhone)?.Value;

                          var userService = context.HttpContext.RequestServices.GetService<IUnitOfWork>();

                          var incomingUser = userService.Repository<User>().GetById(x => x.Id == userId).FirstOrDefault();

                          if (incomingUser == null)
                          {
                              var userToBeAdded = new User
                              {
                                  Id = userId,
                                  Email = email,
                                  FirsName = firstName,
                                  LastName = lastName,
                                  Gender = gender,
                                  DateOfBirth = DateTime.Parse(birthdate),
                                  PhoneNumber = phone ?? " "
                              };

                              userService.Repository<User>().Create(userToBeAdded);

                              var emailService = context.HttpContext.RequestServices.GetService<IEmailSender>();
                              if (emailService != null)
                              {
                                  emailService.SendEmailAsync(userToBeAdded.Email, "Welcome", "Welcome To E-commerce Gjirafa");
                              }
                          }
                          else
                          {
                              var existingUser = userService.Repository<User>().GetById(x => x.Id == userId).FirstOrDefault();
                              existingUser.FirsName = firstName;
                              existingUser.LastName = lastName;
                              existingUser.PhoneNumber = phone ?? " ";

                              userService.Repository<User>().Update(existingUser);
                          }

                          userService.Complete();
                      }
                  };

                  // if token does not contain a dot, it is a reference token
                  options.ForwardDefaultSelector = Selector.ForwardReferenceToken("token");
              });



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Life ETS", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://sso-sts.gjirafa.dev/connect/authorize"),
                TokenUrl = new Uri("https://sso-sts.gjirafa.dev/connect/token"),
                Scopes = new Dictionary<string, string> {
                                              { "life_api", "Life Api" }
                                          }
            }
        }
    });
    c.OperationFilter<AuthorizeCheckOperationFilter>();
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "EcommerceAPI.xml"));
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Stripe Infrastructure
builder.Services.AddStripeInfrastructure(builder.Configuration);


var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


builder.Services.AddHostedService<UpdateElasticBackgroundService>();


builder.Services.AddFluentValidations();
builder.Services.AddEmailSenders(builder.Configuration);

var smtpConfigurations = builder.Configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();
builder.Services.AddSingleton(smtpConfigurations);
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

builder.Services.AddHttpClient();



builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:3000")
            .AllowCredentials();
    });
});

var mapperConfiguration = new MapperConfiguration(
    mc => mc.AddProfile(new AutoMapperConfigurations()));

IMapper mapper = mapperConfiguration.CreateMapper();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton(mapper);
builder.Services.AddDbContext<EcommerceDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddServices();


var pool = new SingleNodeConnectionPool(new Uri("https://localhost:9200"));

var connectionSettings = new ConnectionSettings(pool)
                .BasicAuthentication("elastic", "pfEzK09bAv3=ie56=DFX")
                .CertificateFingerprint("e607c5b0f141794f57bed41248bf36bb3711bed76fa9e526719cf1aeff4968c8");

var client = new ElasticClient(connectionSettings);

builder.Services.AddSingleton(client);

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<ISavedItemService, SavedItemService>();

builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddTransient<PaymentMethodService>();
builder.Services.AddTransient<PaymentIntentService>();
builder.Services.AddScoped<ShoppingCardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DisplayRequestDuration();
        c.DefaultModelExpandDepth(0);
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Life ETS");
        c.OAuthClientId("fb1b97e4-778a-431d-abb1-78bbdca9253b");
        c.OAuthClientSecret("21551351-fc9a-4d8e-8619-8c7e5acb6d47");
        c.OAuthAppName("Ecommerce Project");
        c.OAuthUsePkce();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });

}
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("ClientPermission");


app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/hubs/chat");
});

app.Run();

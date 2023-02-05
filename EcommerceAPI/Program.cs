using AutoMapper;
using Core.Hubs;
using Domain.Entities;
using EcommerceAPI.Extensions;
using EcommerceAPI.Helpers;
using EcommerceAPI.Helpers.EmailSender;
using EcommerceAPI.Infrastructure;
using EcommerceAPI.Workers;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using Persistence;
using Persistence.UnitOfWork.IUnitOfWork;
using Serilog;
using Stripe;
using System.Security.Claims;
using System.Text;
using claims = System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

#region [JWTAuthentication]
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

                          await userService.CompleteAsync();
                      }
                  };

                  // if token does not contain a dot, it is a reference token
                  options.ForwardDefaultSelector = Selector.ForwardReferenceToken("token");
              });


#endregion 


#region[Swagger Gen]
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerceProject", Version = "v1" });
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

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Stripe Infrastructure
builder.Services.AddStripeInfrastructure(builder.Configuration);


#region [Serilog]
var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

#endregion


builder.Services.AddHostedService<UpdateElasticBackgroundService>();
builder.Services.AddFluentValidations();


#region [Email Senders]
var smtpConfigurations = builder.Configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();
builder.Services.AddSingleton(smtpConfigurations);
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();

#endregion

builder.Services.AddEmailSenders(builder.Configuration);


builder.Services.AddHttpClient();


builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

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

#region [Mapper]
var mapperConfiguration = new MapperConfiguration(
    mc => mc.AddProfile(new AutoMapperConfigurations()));

IMapper mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);

#endregion

#region [ElasticSearch]
var pool = new SingleNodeConnectionPool(new Uri("https://localhost:9200"));

var connectionSettings = new ConnectionSettings(pool)
                .BasicAuthentication("elastic", "pfEzK09bAv3=ie56=DFX")
                .CertificateFingerprint("e607c5b0f141794f57bed41248bf36bb3711bed76fa9e526719cf1aeff4968c8");

var client = new ElasticClient(connectionSettings);

#endregion

builder.Services.AddDbContext<EcommerceDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddServices();



builder.Services.AddSingleton(client);

builder.Services.AddTransient<PaymentMethodService>();
builder.Services.AddTransient<PaymentIntentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.ConfigureSwagger();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("ClientPermission");


app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.MapUserEndpoints();

app.Run();
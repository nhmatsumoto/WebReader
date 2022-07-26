using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebReader.API.Database.Context;

namespace WebReader.API
{
    public static class Bootstrap
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection, ConfigurationManager configManager)
        {
            var securityScheme = new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JSON Web Token based security",
            };

            var contact = new OpenApiContact()
            {
                Name = "Nilton H. Matsumoto",
                Email = "hiroyukims@hotmail.com",
                Url = new Uri("https://nhmatsumoto.github.io/")
            };

            var info = new OpenApiInfo()
            {
                Version = "v1",
                Title = "WebReader demo",
                Description = "WebReader is a software for making support to farmers",
                //TermsOfService = new Uri(""),
                Contact = contact
            };

            var securityReq = new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            };

            //ADD SERVICES CONFIGURATION
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen(o =>
            {
                o.SwaggerDoc("v1", info);
                o.AddSecurityDefinition("Bearer", securityScheme);
                o.AddSecurityRequirement(securityReq);
            });
            serviceCollection.AddControllers();
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen();

            serviceCollection.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer("Server=localhost;Database=sa;Trusted_Connection=True;")
            );

            //ADD JWT CONFIGURATION
            serviceCollection.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configManager.GetSection("Jwt:Issuer").Value, 
                    ValidAudience = configManager.GetSection("Jwt:Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(configManager.GetSection("Jwt: Key").Value)),

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });

            serviceCollection.AddControllers();
            serviceCollection.AddEndpointsApiExplorer();
            serviceCollection.AddSwaggerGen();
            serviceCollection.AddAuthorization();

            return serviceCollection;
        }
    }
}

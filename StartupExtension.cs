using GoalTracker.Repository;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using GoalTracker.Repository.Repos;
using GoalTracker.Services.Interfaces;
using GoalTracker.Services.Services;
using GoalTracker.Domain.Interfaces;
using GoalTracker.Services.Security;
using GoalTracker.API.Helpers.Security;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace GoalTracker.API
{
    public static class StartupExtension
    {
        public static IServiceCollection AddAzureDataBaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("AzureConnection");
            services.AddDbContext<DataContext>(options => options.UseSqlServer(connString));
            return services;
        }

        public static IServiceCollection AddCorsForAllOrigins(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder => 
                builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });
            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1.0", new OpenApiInfo { Title = "Main API v1.0", Version = "v1.0" });
            });
            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Versioned API v1.0");

                c.DocumentTitle = "Title Documentation";
                c.DocExpansion(DocExpansion.None);
            });

            return app;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserServices>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISecurePassword, SecurePassword>();
            services.AddScoped<Security>();
            return services;
        }

        public static IServiceCollection AddJwTtoken(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration.GetSection("AppSettings:Token").Value);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });
            return services;
        }
    }
}
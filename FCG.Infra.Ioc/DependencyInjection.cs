using FCG.Application.Interfaces;
using FCG.Application.Mappings;
using FCG.Application.Services;
using FCG.Domain.Entities;
using FCG.Domain.EventSourcing;
using FCG.Domain.Interfaces;
using FCG.Infra.Data.Context;
using FCG.Infra.Data.EventSourcing;
using FCG.Infra.Data.Repositories;
using FCG.Infra.Data.Transactions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FCG.Infra.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    
                    ValidIssuer = configuration["jwt:issuer"],
                    ValidAudience = configuration["jwt:audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["jwt:secretKey"] + "")),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddHttpClient();
            services.Configure<AzureFunctionsOptions>(configuration.GetSection("AzureFunctions"));

            services.AddAutoMapper(typeof(EntitiesToDTOMappingProfile));

            services.AddScoped<IPagamentoRepository, PagamentoRepository>();
            services.AddScoped<IEventStoreRepository, EventStoreRepository>();
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddScoped<IPagamentoService,PagamentoService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();           

            return services;
        }
    }
}

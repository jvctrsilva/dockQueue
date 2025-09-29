﻿using DockQueue.Application.Services;
using DockQueue.Infra.Data.Context;
using DockQueue.Infra.Data.Repositories;
using DockQueue.Infra.Data.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DockQueue.Domain.Interfaces;
using DockQueue.Application.Interfaces;

namespace DockQueue.Infra.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBoxRepository, BoxRepository>();

            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBoxService, BoxService>();
            services.AddScoped<JwtTokenGenerator>();
            services.AddSingleton<ITokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
using System;
using DreamFoodDelivery.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using DreamFoodDelivery.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace DreamFoodDelivery.Data
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            //configure your Data Layer services here
            services.AddDbContext<DreamFoodDeliveryContext>(options => options.UseSqlServer(configuration.GetSection("ConnectionString:Connection").Value));
            services.AddDbContext<UserContext>(options =>options.UseSqlServer(configuration.GetSection("ConnectionString:UserDbConnection").Value));

            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 10;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<UserContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
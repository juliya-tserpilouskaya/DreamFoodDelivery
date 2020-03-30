using System;
using DreamFoodDelivery.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace TrainingProject.Data
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
        {
            //configure your Data Layer services here
            services.AddDbContext<DreamFoodDeliveryContext>(options => options.UseSqlServer(configuration.GetSection("ConnectionString:Connection").Value));
            return services;
        }
    }
}
﻿using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DreamFoodDelivery.Data;
using AutoMapper;
using DreamFoodDelivery.Domain.Logic.InterfaceServices;
using DreamFoodDelivery.Domain.Logic.Services;

namespace DreamFoodDelivery.Domain.Logic
{
    public static class DomainLogicExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataServices(configuration);
            //configure your Domain Logic Layer services here

            services.AddAutoMapper(typeof(MapperLogicProfile));

            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IImageInterface, ImageService>();
            services.AddTransient<IEmailSenderService, EmailSenderService>();
            services.AddTransient<IEmailBuilder, EmailBuilder>();

            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<ITokenValidator, JwtTokenValidator>();

            services.Configure<AuthMessageSenderOptions>(configuration);

            return services;
        }
    }
}
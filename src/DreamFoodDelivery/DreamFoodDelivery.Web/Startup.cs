using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DreamFoodDelivery.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using DreamFoodDelivery.Domain.Logic;
using NSwag.Generation.Processors.Security;
using NSwag;
using FluentValidation.AspNetCore;
using DreamFoodDelivery.Domain.Logic.Validation;

namespace DreamFoodDelivery.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDomainServices(Configuration);

            var tokenSecret = new TokenSecret();
            Configuration.Bind(nameof(tokenSecret), tokenSecret);
            services.AddSingleton(tokenSecret);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSecret.SecretString)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = false,
                        ValidateLifetime = true
                    };
                });

            services.AddOpenApiDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Title = "~Dream food for you~";
                    document.Info.Description = "Intership ASP.NET Core web API";
                    document.Info.Version = "v0.0.1";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Yuliya Tserpilouskaya",
                        Email = "yuliya.tserpilouskaya@gmail.com",
                        Url = string.Empty
                    };
                };
                config.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the bearer scheme",
                        Name = "Authorization",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Type = OpenApiSecuritySchemeType.ApiKey
                    }));
                config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("Bearer"));
            });

            services.AddControllers().AddFluentValidation(fluentValidation =>
            {
                fluentValidation.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                fluentValidation.RegisterValidatorsFromAssemblyContaining<DishToAddValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<DishToUpdateValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<TagToAddValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<TagToUpdateValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<CommentToAddValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<CommentToUpdateValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<OrderToAddValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<OrderToStatusUpdateValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<OrderToUpdateValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<UserPasswordToChangeValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<UserToUpdateValidation>();
            });
            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi().UseSwaggerUi3();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseCors(builder => builder.WithOrigins("http://localhost:4200"));

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
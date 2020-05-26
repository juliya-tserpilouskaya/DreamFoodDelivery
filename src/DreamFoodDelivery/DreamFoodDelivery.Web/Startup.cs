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
using Microsoft.Extensions.FileProviders;
using System.IO;
using DreamFoodDelivery.Common.�onstants;

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
            //In-Memory
            services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
            services.AddSession();
            var tokenSecret = new TokenSecret();
            Configuration.Bind(nameof(tokenSecret), tokenSecret);
            services.AddSingleton(tokenSecret);

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                    document.Info.Title = ApiDocConstants.TITLE;
                    document.Info.Description = ApiDocConstants.DESCRIPTION;
                    document.Info.Version = ApiDocConstants.VERSION;
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = ApiDocConstants.CONTACT_NAME,
                        Email = ApiDocConstants.CONTACT_EMAIL,
                        Url = ApiDocConstants.CONTACT_URL
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

            services.AddControllers()
                .AddNewtonsoftJson()
                .AddFluentValidation(fluentValidation =>
            {
                fluentValidation.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                fluentValidation.RegisterValidatorsFromAssemblyContaining<DishToBasketAddValidation>();
                fluentValidation.RegisterValidatorsFromAssemblyContaining<DishByCostValidation>();
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
                fluentValidation.RegisterValidatorsFromAssemblyContaining<SearchValidation>();
            });

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetIsOriginAllowedToAllowWildcardSubdomains());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseOpenApi().UseSwaggerUi3();
            }
            app.UseSession(); // use this before .UseEndpoints
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
                RequestPath = "/Images"
            });

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
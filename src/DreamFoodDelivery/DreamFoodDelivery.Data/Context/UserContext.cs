using DreamFoodDelivery.Common;
using DreamFoodDelivery.Data.Configurations;
using DreamFoodDelivery.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Context
{
    public class UserContext : IdentityDbContext<AppUser>
    {
        public UserContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder)); //Is it necessary?
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = AppIdentityConstants.ADMIN,
                    NormalizedName = AppIdentityConstants.ADMIN.ToUpper()
                },
                new IdentityRole
                {
                    Name = AppIdentityConstants.USER,
                    NormalizedName = AppIdentityConstants.USER.ToUpper()
                });
        }
    }
}

using DreamFoodDelivery.Data.Configurations;
using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Context
{
    public class DreamFoodDeliveryContext : DbContext
    {
        public DreamFoodDeliveryContext()
        {

        }
        public DreamFoodDeliveryContext(DbContextOptions<DreamFoodDeliveryContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new DishConfiguration());
            modelBuilder.ApplyConfiguration(new DishTagConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new BasketConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            //modelBuilder.ApplyConfiguration(new UserInfoConfiguration());
        }
        public DbSet<DishDB> Dishes { get; set; }
        public DbSet<DishTagDB> DishTags { get; set; }
        public DbSet<TagDB> Tags { get; set; }
        public DbSet<BasketDB> Baskets { get; set; }
        public DbSet<CommentDB> Comments { get; set; }
        public DbSet<OrderDB> Orders { get; set; }
        public DbSet<UserDB> Users { get; set; }
        //public DbSet<UserInfoDB> UsersInfo { get; set; }
    }
}

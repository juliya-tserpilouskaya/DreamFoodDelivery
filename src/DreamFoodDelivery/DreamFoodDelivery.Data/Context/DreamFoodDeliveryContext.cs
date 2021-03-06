﻿using DreamFoodDelivery.Data.Configurations;
using DreamFoodDelivery.Data.Configurations.User;
using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DreamFoodDelivery.Data.Context
{
    public class DreamFoodDeliveryContext : DbContext
    {
        public DreamFoodDeliveryContext(DbContextOptions<DreamFoodDeliveryContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new DishConfiguration());
            modelBuilder.ApplyConfiguration(new DishTagConfiguration());
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new BasketConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new BasketDishConfiguration());
            modelBuilder.ApplyConfiguration(new RatingConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfig());
        }
        public DbSet<DishDB> Dishes { get; set; }
        public DbSet<DishTagDB> DishTags { get; set; }
        public DbSet<TagDB> Tags { get; set; }
        public DbSet<BasketDB> Baskets { get; set; }
        public DbSet<ReviewDB> Reviews { get; set; }
        public DbSet<OrderDB> Orders { get; set; }
        public DbSet<UserDB> Users { get; set; }
        public DbSet<BasketDishDB> BasketDishes { get; set; }
        public DbSet<RatingDB> Rating { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AppUser ||
                    entry.Entity is UserDB ||
                    entry.Entity is BasketDB ||
                    entry.Entity is BasketDishDB ||
                    entry.Entity is RatingDB ||
                    entry.Entity is RefreshToken)
                {
                    continue;
                };

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues["IsDeleted"] = false;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsDeleted"] = true;
                        break;
                }
            }
        }
    }
}

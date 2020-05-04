using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class BasketDishConfiguration : IEntityTypeConfiguration<BasketDishDB>
    {
        public void Configure(EntityTypeBuilder<BasketDishDB> builder)
        {
            builder.ToTable("BasketDishes");
            builder.HasKey(i => i.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

            builder.Property<bool>("IsDeleted");
            builder.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false);
        }
    }
}

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
            builder.ToTable("BasketConnection");
            builder.HasKey(i => i.ConnectionId);
            builder.Property(_ => _.ConnectionId).ValueGeneratedOnAdd();
        }
    }
}

﻿using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class BasketConfiguration : IEntityTypeConfiguration<BasketDB>
    {
        public void Configure(EntityTypeBuilder<BasketDB> builder)
        {
            builder.ToTable("Basket");
            builder.HasKey(i => i.Id);
            builder.Property(_ => _.Id)/*.ValueGeneratedOnAdd()*/;

            builder.HasOne(_ => _.User).WithOne(_ => _.Basket)
                   .HasForeignKey<UserDB>(_ => _.BasketId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(i => i.Id).HasColumnName("Id");

            builder.Property<bool>("IsDeleted");
            builder.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false);
        }
    }
}

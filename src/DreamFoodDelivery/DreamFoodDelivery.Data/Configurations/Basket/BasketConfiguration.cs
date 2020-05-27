using DreamFoodDelivery.Data.Models;
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
            builder.Property(_ => _.Id);

            builder.HasOne(_ => _.User).WithOne(_ => _.Basket)
                   .HasForeignKey<UserDB>(_ => _.BasketId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Id).HasColumnName("Id");
        }
    }
}

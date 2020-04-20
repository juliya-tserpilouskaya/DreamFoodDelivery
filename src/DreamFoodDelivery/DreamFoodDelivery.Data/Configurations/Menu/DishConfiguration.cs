using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class DishConfiguration : IEntityTypeConfiguration<DishDB>
    {
        public void Configure(EntityTypeBuilder<DishDB> builder)
        {
            builder.ToTable("Dish");
            builder.HasKey(i => i.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();
            //
            //builder.HasOne(_ => _.Basket).WithMany(_ => _.Dishes)
            //       .HasForeignKey(_ => _.BasketId)
            //       .OnDelete(DeleteBehavior.Restrict);
            //
            builder.HasMany(_ => _.DishTags).WithOne(_ => _.Dish).HasForeignKey(_ => _.DishId).OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Id).HasColumnName("Id");
            //builder.Property(i => i.Name).HasColumnName("Name").IsRequired().HasMaxLength(63);
            //builder.Property(i => i.Category).HasColumnName("Category").IsRequired().HasMaxLength(31);
            //builder.Property(i => i.Сomposition).HasColumnName("Сomposition").IsRequired().HasMaxLength(255);
            //builder.Property(i => i.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
            //builder.Property(i => i.Weigh).HasColumnName("Weigh").IsRequired().HasMaxLength(127);
            //builder.Property(i => i.Cost).HasColumnName("Cost").IsRequired();
            //builder.Property(i => i.Sale).HasColumnName("Sale").IsRequired();
        }
    }
}

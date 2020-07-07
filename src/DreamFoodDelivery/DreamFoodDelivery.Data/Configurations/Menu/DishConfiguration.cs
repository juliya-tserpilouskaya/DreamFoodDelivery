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
            
            builder.HasMany(_ => _.DishTags).WithOne(_ => _.Dish).HasForeignKey(_ => _.DishId).OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Id).HasColumnName("Id");
            builder.Property(i => i.Name).HasColumnName("Name").IsRequired().HasMaxLength(63);
            builder.Property(i => i.Composition).HasColumnName("Сomposition").IsRequired().HasMaxLength(255);
            builder.Property(i => i.Description).HasColumnName("Description").IsRequired().HasMaxLength(255);
            builder.Property(i => i.Weight).HasColumnName("Weight").IsRequired().HasMaxLength(127);
            builder.Property(i => i.Price).HasColumnName("Price").IsRequired();
            builder.Property(i => i.Sale).HasColumnName("Sale").IsRequired();
            builder.Property<bool>("IsDeleted");
            builder.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false);
        }
    }
}

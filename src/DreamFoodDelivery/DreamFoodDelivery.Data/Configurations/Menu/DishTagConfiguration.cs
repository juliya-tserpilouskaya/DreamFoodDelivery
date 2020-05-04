using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    class DishTagConfiguration : IEntityTypeConfiguration<DishTagDB>
    {
        public void Configure(EntityTypeBuilder<DishTagDB> builder)
        {
            builder.HasKey(_ => new { _.TagId, _.DishId });
            builder.HasOne(_ => _.Tag).WithMany(_ => _.DishTags).HasForeignKey(_ => _.TagId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(_ => _.Dish).WithMany(_ => _.DishTags).HasForeignKey(_ => _.DishId).OnDelete(DeleteBehavior.Restrict);

            builder.Property<bool>("IsDeleted");
            builder.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false);
        }
    }
}

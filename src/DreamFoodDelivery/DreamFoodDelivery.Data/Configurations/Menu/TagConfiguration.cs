using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using DreamFoodDelivery.Data.Models;

namespace DreamFoodDelivery.Data.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<TagDB>
    {
        public void Configure(EntityTypeBuilder<TagDB> builder)
        {
            builder.ToTable("Tag");
            builder.HasKey(i => i.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

            builder.HasMany(_ => _.DishTags).WithOne(_ => _.Tag).HasForeignKey(_ => _.TagId).OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Id).HasColumnName("Id");

            builder.Property<bool>("IsDeleted");
            builder.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false);
        }
    }
}

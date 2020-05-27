using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<CommentDB>
    {
        public void Configure(EntityTypeBuilder<CommentDB> builder)
        {
            builder.ToTable("Review");
            builder.HasKey(i => i.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

            builder.HasOne(_ => _.User).WithMany(_ => _.Comments)
                   .HasForeignKey(_ => _.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(_ => _.Order).WithOne(_ => _.Comment)
                   .HasForeignKey<OrderDB>(_ => _.CommentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Id).HasColumnName("Id");
            builder.Property(i => i.Headline).HasColumnName("Headline").HasMaxLength(63);
            builder.Property(i => i.Rating).HasColumnName("Rating");
            builder.Property(i => i.Content).HasColumnName("Content").HasMaxLength(511);
            builder.Property<bool>("IsDeleted");
            builder.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false);
        }
    }
}

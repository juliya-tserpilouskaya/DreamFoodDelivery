using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderDB>
    {
        public void Configure(EntityTypeBuilder<OrderDB> builder)
        {
            builder.ToTable("Order");
            builder.HasKey(i => i.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

            builder.HasOne(_ => _.User).WithMany(_ => _.Orders)
                   .HasForeignKey(_ => _.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(_ => _.Comment).WithOne(_ => _.Order)
                   .HasForeignKey<CommentDB>(_ => _.OrderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Id).HasColumnName("Id");
            builder.Property(i => i.IsInfoFromProfile).HasColumnName("IsInfoFromProfile");
            builder.Property(i => i.Address).HasColumnName("Address").HasMaxLength(255);
            builder.Property(i => i.PersonalDiscount).HasColumnName("PersonalDiscount").HasMaxLength(7);
            builder.Property(i => i.PhoneNumber).HasColumnName("PhoneNumber").HasMaxLength(31);
            builder.Property(i => i.Name).HasColumnName("Name").HasMaxLength(63);
            builder.Property(i => i.OrderCost).HasColumnName("FinaleCost");
            builder.Property(i => i.ShippingCost).HasColumnName("ShippingСost");
            builder.Property(i => i.Status).HasColumnName("Status").HasMaxLength(31);

            builder.Property<bool>("IsDeleted");
            builder.HasQueryFilter(post => EF.Property<bool>(post, "IsDeleted") == false);
        }
    }
}

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
            builder.Property(i => i.IsInfoFromProfile).HasColumnName("IsInfoFromProfile").IsRequired();
            builder.Property(i => i.Address).HasColumnName("Address").IsRequired().HasMaxLength(255);
            builder.Property(i => i.PersonalDiscount).HasColumnName("PersonalDiscount").IsRequired().HasMaxLength(7);
            builder.Property(i => i.PhoneNumber).HasColumnName("PhoneNumber").IsRequired().HasMaxLength(31); //In Germany, numbers > 16
            builder.Property(i => i.Name).HasColumnName("Name").IsRequired().HasMaxLength(63);
            builder.Property(i => i.FinaleCost).HasColumnName("FinaleCost").IsRequired();
            builder.Property(i => i.ShippingСost).HasColumnName("ShippingСost").IsRequired();
            builder.Property(i => i.Status).HasColumnName("Status").IsRequired().HasMaxLength(31);
            builder.Property(i => i.Paid).HasColumnName("Paid").IsRequired();
        }
    }
}

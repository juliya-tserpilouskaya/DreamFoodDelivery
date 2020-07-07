using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserDB>
    {
        public void Configure(EntityTypeBuilder<UserDB> builder)
        {
            builder.ToTable("User");
            builder.HasKey(i => i.Id);
            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

            builder.Property(_ => _.IdFromIdentity).IsRequired();
            builder.HasIndex(_ => _.IdFromIdentity).IsUnique();

            builder.HasOne(_ => _.Basket).WithOne(_ => _.User)
                   .HasForeignKey<BasketDB>(_ => _.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(_ => _.Orders).WithOne(_ => _.User)
                   .HasForeignKey(_ => _.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(_ => _.Reviews).WithOne(_ => _.User)
                   .HasForeignKey(_ => _.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(_ => _.RefreshTokens).WithOne(_ => _.User)
                   .HasForeignKey(_ => _.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Id).HasColumnName("Id");
            builder.Property(i => i.IdFromIdentity).HasColumnName("Identity ID");
        }
    }
}

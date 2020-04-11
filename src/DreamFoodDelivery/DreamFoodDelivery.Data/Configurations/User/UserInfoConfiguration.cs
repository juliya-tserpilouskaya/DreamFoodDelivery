//using DreamFoodDelivery.Data.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace DreamFoodDelivery.Data.Configurations
//{
//    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfoDB>
//    {
//        public void Configure(EntityTypeBuilder<UserInfoDB> builder)
//        {
//            builder.ToTable("User_infomation");
//            builder.HasKey(i => i.Id);
//            builder.Property(_ => _.Id).ValueGeneratedOnAdd();

//            //builder.HasOne(_ => _.User).WithOne(_ => _.UserInfo)
//            //       .HasForeignKey<UserDB>(_ => _.UserInfoId)
//            //       .OnDelete(DeleteBehavior.Restrict);

//            builder.Property(i => i.Id).HasColumnName("Id");
//            builder.Property(i => i.UserId).HasColumnName("User_id")/*.IsRequired()*/;
//            builder.Property(i => i.Name).HasColumnName("Name")/*.IsRequired()*/.HasMaxLength(63);
//            builder.Property(i => i.Surname).HasColumnName("Surname")/*.IsRequired()*/.HasMaxLength(63);
//            builder.Property(i => i.PhoneNumber).HasColumnName("PhoneNumber")/*.IsRequired()*/.HasMaxLength(31); //In Germany, numbers > 16
//            builder.Property(i => i.Address).HasColumnName("Address").HasMaxLength(255);
//            builder.Property(i => i.PersonalDiscount).HasColumnName("PersonalDiscount");
//            //builder.Property(i => i.User).HasColumnName("User");
//        }
//    }
//}

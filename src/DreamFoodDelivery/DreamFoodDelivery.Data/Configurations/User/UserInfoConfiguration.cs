using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfoDB>
    {
        public void Configure(EntityTypeBuilder<UserInfoDB> builder)
        {
            throw new NotImplementedException();
        }
    }
}

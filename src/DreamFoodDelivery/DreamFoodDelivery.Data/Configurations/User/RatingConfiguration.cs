using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations.User
{
    public class RatingConfiguration : IEntityTypeConfiguration<RatingDB>
    {
        public void Configure(EntityTypeBuilder<RatingDB> builder)
        {
            builder.ToTable("Rating");
            builder.HasKey(i => i.Id);
        }
    }
}

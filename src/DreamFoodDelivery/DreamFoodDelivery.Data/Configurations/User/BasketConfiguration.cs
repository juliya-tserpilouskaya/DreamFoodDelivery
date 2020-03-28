using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class BasketConfiguration : IEntityTypeConfiguration<BasketDB>
    {
        public void Configure(EntityTypeBuilder<BasketDB> builder)
        {
            throw new NotImplementedException();
        }
    }
}

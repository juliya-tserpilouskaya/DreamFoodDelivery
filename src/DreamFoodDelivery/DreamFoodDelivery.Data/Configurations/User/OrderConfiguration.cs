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
            throw new NotImplementedException();
        }
    }
}

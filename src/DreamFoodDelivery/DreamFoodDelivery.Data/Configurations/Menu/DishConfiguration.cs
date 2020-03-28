using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class DishConfiguration : IEntityTypeConfiguration<DishDB>
    {
        public void Configure(EntityTypeBuilder<DishDB> builder)
        {
            throw new NotImplementedException();
        }
    }
}

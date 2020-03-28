using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using DreamFoodDelivery.Data.Models;

namespace DreamFoodDelivery.Data.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<TagDB>
    {
        public void Configure(EntityTypeBuilder<TagDB> builder)
        {
            throw new NotImplementedException();
        }
    }
}

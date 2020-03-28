using DreamFoodDelivery.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Data.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<CommentDB>
    {
        public void Configure(EntityTypeBuilder<CommentDB> builder)
        {
            throw new NotImplementedException();
        }
    }
}

using AutoMapper;
using DreamFoodDelivery.Data.Models;
using DreamFoodDelivery.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamFoodDelivery.Domain.Logic
{
    class MapperLogicProfile : Profile
    {
        public MapperLogicProfile()
        {
            CreateMap<CommentDB, CommentDTO_View>().ReverseMap();
        }
    }
}
